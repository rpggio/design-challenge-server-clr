using System;
using System.Text.RegularExpressions;
using DCS.Contracts;
using DCS.Contracts.Entities;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using DCS.ServerRuntime.Services.GitblitApi;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class ChallengeInitializer : IHandleMessages<InitializeChallenge>    
    {
        private const string InitStage = "stage000";
        private const string FirstStage = "stage001";
        private readonly ILog _log;
        private readonly AppSettings _settings;
        private readonly ScmClient _scmClient;
        private readonly DcsFilesystem _filesystem;
        private readonly IBus _bus;
        private readonly EntitiesRoot _entities;
        private readonly Repositories _repositories;
        private readonly DcsScm _scm;
        private readonly GitblitClient _gitblit;

        public ChallengeInitializer(ILog log, AppSettings settings, ScmClient scmClient, DcsFilesystem filesystem,
            IBus bus, EntitiesRoot entities, DcsScm scm, Repositories repositories, GitblitClient gitblit)
        {
            _log = log;
            _settings = settings;
            _scmClient = scmClient;
            _filesystem = filesystem;
            _bus = bus;
            _entities = entities;
            _scm = scm;
            _repositories = repositories;
            _gitblit = gitblit;
        }

        public void Handle(InitializeChallenge message)
        {
            if (string.IsNullOrWhiteSpace(message.ChallengeName)
                || string.IsNullOrWhiteSpace(message.Repository)
                || string.IsNullOrWhiteSpace(message.StarterName)
                || string.IsNullOrWhiteSpace(message.Username))
            {
                _log.WarnFormat("Initialize message is invalid. Ignoring.");
                return;
            }

            var user = _entities.Users.Get(message.Username);

            if (user == null)
            {
                _log.InfoFormat("Could not find user {0}. Ignoring message.", message.Username);
                return;
            }

            var challengeDir = _filesystem.ChallengesSource.Challenges[message.ChallengeName];
            if (!challengeDir.Exists())
            {
                throw new Exception(string.Format("Could not find challenge {0}", message.ChallengeName));
            }

            bool newRepo = false;
            var repo = _repositories.Get(message.Repository);
            if (repo == null)
            {
                newRepo = true;
            }

            var buildDir = _filesystem.CreateBuildDir(message.Repository);
            try
            {
                var repoUrl = _scm.GetRepoUrl(message.Repository, _settings.Git.SystemUser, true);

                if (newRepo)
                {
                    repo = new RepositoryEntity()
                    {
                        Id = Guid.NewGuid(),
                        Challenge = message.ChallengeName,
                        Name = message.Repository,
                        UserId = user.Id
                    };
                    
                    _gitblit.CreateRepo(new GitblitRepository()
                    {
                        name = repo.Name,
                        description = "{0} - {1}".FormatFrom(user.Username, repo.Challenge),
                        owner = _settings.Git.SystemUser.Username,
                        accessRestriction = "PUSH"
                    });
                    
                    _repositories.Save(repo);

                    if (
                        !_scmClient.Init(buildDir.Path)
                        || !_scmClient.RemoteOrigin(buildDir.Path, repoUrl)
                        )
                    {
                        throw new Exception("Failed to initialize challenge");
                    }
                }
                else
                {
                    if (!Clone(message, repoUrl, buildDir))
                    {
                        _log.ErrorFormat("Failed to clone {0}", repoUrl);
                        return;
                    }
                }

                //todo: use the expand directory for the next three lines

                // copy solution init
                challengeDir.Stage(InitStage)
                    .Solutions[message.StarterName]
                    .CopyContentsInto(buildDir);

                // copy features init
                challengeDir.Stage(InitStage)
                    .Features
                    .CopyContentsInto(buildDir.Features);

                // copy first stage features
                challengeDir.Stage(FirstStage).Features
                    .CopyContentsInto(buildDir.Features);

                if (!_scmClient.SetUser(buildDir.Path, _settings.Git.SystemUser)
                    || !_scmClient.AddCommitPush(buildDir.Path,
                        string.Format("Initializing challenge {0}", message.ChallengeName)))
                {
                    throw new Exception("Failed to initialize challenge");
                }

                // add repo permissions

                var gitblitUser = _gitblit.GetUser(user.Username);
                var repoNameWithSuffix = repo.Name.EnsureEndsWith(".git");
                if (!gitblitUser.repositories.Contains(repoNameWithSuffix))
                {
                    gitblitUser.repositories.Add(repoNameWithSuffix);
                }
                gitblitUser.permissions[repoNameWithSuffix] = "RW";
                _gitblit.EditUser(gitblitUser);

                // save user update to DB

                user.AddChallenge(message.ChallengeName, repo.Id);
                _entities.Users.Save(user);

                _bus.Publish(new NotifyUser
                {
                    UserId = user.Id,
                    Subject = "Welcome to CodeFlight",
                    Body =
                        "<p>A CodeFlight account has been created for you.</p> <p>Your challenge has been set up in repository {0}.</p> <p>Username: <b>{0}</b> <br>Password: <b>{1}</b> </p>"
                            .FormatFrom(repo.Name, user.Username, user.Password)
                });
            }
            finally
            {
                _bus.Publish(new DeleteDirectory {Path = buildDir.Path});
            }
        }

        private bool Clone(InitializeChallenge message, string repoUrl, StageBuildDirectory buildDir)
        {
            if (!_scmClient.Clone(repoUrl, buildDir.Path))
            {
                throw new Exception("Failed to clone dir");
            }

            // clean up any existing files
            try
            {
                foreach (var dir in buildDir.Directories())
                {
                    if (Regex.IsMatch(dir.Name(), @"\.?git"))
                    {
                        continue;
                    }
                    dir.Delete();
                }
                buildDir.DeleteFiles();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Could not reset files in cloned directory: {0}", ex.Summary());
                _bus.Publish(FailedMessage.Create(message));
                return false;
            }
            return true;
        }
    }
}