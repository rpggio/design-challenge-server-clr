using DCS.Contracts;
using DCS.Core.IO;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;
using Rebus;

namespace DCS.Services.Bus
{
    public class StagePromoter : IHandleMessages<UserBuildComplete>
    {
        private readonly ILog _log;
        private readonly IBus _bus;
        private readonly DcsFilesystem _filesystem;
        private readonly ExpandedChallenges _expandedChallenges;
        private readonly EntitiesRoot _entities;
        private readonly ScmClient _scm;
        private readonly AppSettings _settings;

        public StagePromoter(ILog log, IBus bus, DcsFilesystem filesystem, ExpandedChallenges expandedChallenges,
            EntitiesRoot entities, ScmClient scm, AppSettings settings)
        {
            _log = log;
            _bus = bus;
            _filesystem = filesystem;
            _expandedChallenges = expandedChallenges;
            _entities = entities;
            _scm = scm;
            _settings = settings;
        }

        public void Handle(UserBuildComplete message)
        {
            if (!message.Passed)
            {
                return;
            }

            PromoteStage(message);

            _bus.Publish(new DeleteDirectory {Path = message.BuildDir});
        }

        private void PromoteStage(UserBuildComplete message)
        {
            var stageBuildDir = new StageBuildDirectory(message.BuildDir);
            var user = _entities.Users.Get(message.UserId);

            if (user == null)
            {
                _log.WarnFormat("User not found: {0}. Aborting stage promotion.", message.UserId);
            }

            var challenge = user.GetChallenge(message.ChallengeName);

            var nextStage = challenge.Stage + 1;
            var nextStageSourceDir = _filesystem.ChallengesSource.Challenges[challenge.Name]
                .Stage(nextStage.ToString());
            if (!nextStageSourceDir.Exists())
            {
                _log.InfoFormat("User {0} has completed all stages!", user.Username);

                _bus.Publish(new NotifyUser
                {
                    UserId = user.Id,
                    Subject = string.Format("{0}: You have completed all of the stages!", challenge.Name),
                });

                return;
            }

            var nextStageExpandedDir = _expandedChallenges.GetExpandedChallenge(challenge.Name)
                .Stage(challenge.Stage.Name);
            var buildFeaturesDir = stageBuildDir.Directory("features");

            buildFeaturesDir.DeleteContents();
            nextStageExpandedDir.Directory("features").CopyContentsInto(buildFeaturesDir);

            _scm.SetUser(stageBuildDir.Path, _settings.Git.SystemUser);
            _scm.AddCommitPush(buildFeaturesDir.Path, string.Format("Advancing to stage {0}", nextStage.FriendlyName));

            challenge.StageNumber = nextStage.Number;
            _entities.Users.Save(user);

            _bus.Publish(new NotifyUser
            {
                UserId = user.Id,
                Subject = string.Format("{0}: You have advanced to {1}", challenge.Name, nextStage.FriendlyName),
                Body = "The tests for the next stage have been pushed to your Git repository."
            });
        }
    }
}