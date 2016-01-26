using System;
using System.Collections.Generic;
using System.Linq;
using DCS.Contracts;
using DCS.Core.IO;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services.FileSystem;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent(RegisterWith.SingleInstance)]
    public class ExpandedChallenges
    {
        private readonly DcsFilesystem _filesystem;

        private static readonly Dictionary<string, bool> _expandedChallenges =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        public ExpandedChallenges(DcsFilesystem filesystem)
        {
            _filesystem = filesystem;
        }

        public ChallengeSourceDirectory GetExpandedChallenge(string challenge)
        {
            lock (_expandedChallenges)
            {
                if (!_expandedChallenges.ContainsKey(challenge))
                {
                    ExpandStages(challenge);
                    _expandedChallenges.Add(challenge, true);
                }
                return _filesystem.Expand.Challenges[challenge];
            }
        }

        public StageSourceDirectory ExpandStages(string challenge, Stage toStage = null)
        {
            var challengeSource = _filesystem.ChallengesSource.Challenges[challenge];
            if (toStage == null)
            {
                var lastStageDir = challengeSource.Stages.Last(s => 
                    s.Name().StartsWith("stage", StringComparison.OrdinalIgnoreCase));
                toStage = Stage.Parse(lastStageDir.Name());
            }

            var challengeTarget = _filesystem.Expand.Challenges[challenge];
            StageSourceDirectory priorTarget = null;
            for (var stage = Stage.Zero; stage.Number <= toStage.Number; stage = stage + 1)
            {
                var source = challengeSource.Stage(stage);
                var target = challengeTarget.Stage(stage);

                target.DeleteContents();

                if (priorTarget != null)
                {
                    priorTarget.CopyContentsInto(target);
                }

                source.CopyContentsInto(target);
                priorTarget = target;
            }

            return priorTarget;
        }
    }
}