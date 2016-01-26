using System;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;
using Rebus;
using ServiceStack.Text;

namespace DCS.UserServices.Bus
{
    public class PushAssessment : IHandleMessages<UserCommitPush>, IUserAgentConsumer
    {
        private readonly ILog _log;
        private readonly AppSettings _settings;
        private readonly IBus _bus;
        private readonly Users _users;
        private readonly Repositories _repositories;
        private readonly Commits _commits;
        private readonly AssessmentService _assessmentService;
        private readonly DcsFilesystem _filesystem;
        private readonly DcsScm _dcsScm;
        private readonly ScmClient _scm;

        public PushAssessment(ILog log, AppSettings settings, IBus bus, Users users, Repositories repositories, 
            Commits commits, AssessmentService assessmentService, DcsFilesystem filesystem, DcsScm dcsScm, ScmClient scm)
        {
            _log = log;
            _settings = settings;
            _bus = bus;
            _users = users;
            _repositories = repositories;
            _commits = commits;
            _assessmentService = assessmentService;
            _filesystem = filesystem;
            _dcsScm = dcsScm;
            _scm = scm;
        }


        public void Handle(UserCommitPush message)
        {
            if (message.Repository.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
            {
                message.Repository = message.Repository.TrimRight(4);
            }

            _log.InfoFormat("Received push to [{0}] by [{1}]", message.Repository, message.Username);

            var user = _users.Get(message.Username);

            if (user == null)
            {
                _log.InfoFormat("Could not find user {0}. Ignoring.", message.Username);
                return;
            }

            var repository = _repositories.Get(message.Repository);

            if (repository == null)
            {
                _log.InfoFormat("Could not find repository {0}. Ignoring.", message.Repository);
                return;
            }

            var commit = new CommitEntity
            {
                Id = message.CommitId,
                CommittedAt = DateTime.UtcNow,
                CommitMessage = message.Message,
                Username = message.Username,
                UserId = user.Id,
                RepositoryName = message.Repository,
                RepositoryId = repository.Id,
                Challenge = repository.Challenge,
            };

            var userChallenge = user.GetChallenge(repository.Challenge);
            if (userChallenge == null)
            {
                _log.InfoFormat("Could not find user challenge for {0}. Ignoring.", repository.Challenge);
                return;
            }

            commit.CurrentStageNumber = userChallenge.Stage.Number;

            var existingCommit = _commits.Get(commit.Id);
            if (existingCommit == null)
            {
                _commits.Insert(commit);
            }
            else
            {
                _log.WarnFormat("Received duplicate commit for {0}: . Original date: {1}, new date: {2}",
                    existingCommit.ShortId,
                    existingCommit.CommittedAt,
                    commit.CommittedAt
                    );
            }

            if (IsSystemUser(commit.Username))
            {
                _log.DebugFormat("Ignoring commit from system user {0}", commit.Username);
                return;
            }

            _log.DebugFormat("Creating build dir for {0}", repository.Name);
            var stageBuildDir = _filesystem.CreateBuildDir(repository.Name);

            _log.DebugFormat("Cloning repo {0}", repository.Name);
            if (_scm.Clone(_dcsScm.GetRepoUrl(repository.Name, _settings.Git.SystemUser, true), stageBuildDir.Path))
            {
                _log.DebugFormat("checked out repo to {0}", stageBuildDir);
            }
            else
            {
                string error = string.Format("git clone failed on {0}", repository.Name);
                _log.Error(error);
                throw new Exception(error);
            }

            _log.DebugFormat("Sending challenge '{0}' to assessment service", userChallenge.Name);
            var result = _assessmentService.Assess(userChallenge.Name, userChallenge.Stage, stageBuildDir);
            _log.Debug(result.Dump().Left(200));

            _commits.UpdateResult(
                commit.Id,
                result.Outcome,
                result.Message,
                result.TestOutputFormat,
                result.TestOutput,
                result.BuildOutput);

            _log.InfoFormat("Build of [{0}] for user [{1}] result: {2}",
                commit.RepositoryName,
                user.Username,
                result.Outcome);

            if (result.Outcome == AssessmentOutcome.Unknown)
            {
                throw new Exception("Unknown assessment outcome");
            }

            if (result.Outcome != AssessmentOutcome.Success)
            {
                _log.InfoFormat("Assessment of {0} failed: ({1}) {2}",
                    stageBuildDir,
                    result.Outcome,
                    result.BuildOutput);
                _bus.Publish(new NotifyUser
                {
                    UserId = user.Id,
                    Subject = "Submission failed: " + GetSubject(result.Outcome),
                    Body = result.Message + Environment.NewLine + result.BuildOutput.Left(2000)
                });
            }

            var completeMessage = new UserBuildComplete
            {
                Passed = result.Outcome == AssessmentOutcome.Success,
                UserId = user.Id,
                CommitId = commit.Id,
                FromStage = commit.CurrentStageNumber.ToString(),
                ChallengeName = repository.Challenge,
                Repository = message.Repository,
                Output = result.BuildOutput,
                BuildDir = stageBuildDir.Path
            };
            _bus.Publish(completeMessage);
        }

        private static string GetSubject(AssessmentOutcome outcome)
        {
            switch (outcome)
            {
                case AssessmentOutcome.Unknown:
                    return "Unkown outcome";
                case AssessmentOutcome.Success:
                    return "Assessment success";
                case AssessmentOutcome.InvalidContent:
                    return "Submission had invalid content";
                case AssessmentOutcome.BuildFailure:
                    return "Build failed";
                case AssessmentOutcome.SolutionFailure:
                    return "Solution startup failed";
                case AssessmentOutcome.TestFailure:
                    return "Tests failed";
                case AssessmentOutcome.SystemBusy:
                    return "System busy";
                default:
                    throw new ArgumentOutOfRangeException("outcome");
            }
        }

        private bool IsSystemUser(string username)
        {
            return username.Equals(_settings.Git.AdminUser.Username, StringComparison.OrdinalIgnoreCase)
                   || username.Equals(_settings.Git.SystemUser.Username, StringComparison.OrdinalIgnoreCase);
        }
    }
}