using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;

namespace DCS.Console.Commands
{
    [Description("Build stage contents from tip relative to prior stage")]
    public class SeedStageCommand : ConsoleCommandBase
    {
        private readonly ILog _log;
        private readonly DcsFilesystem _dcsFiles;
        private readonly ExpandedChallenges _expandedChallenges;
        private readonly Shell _shell;

        public SeedStageCommand(ILog log, DcsFilesystem dcsFiles, ExpandedChallenges expandedChallenges, Shell shell)
        {
            _log = log;
            _dcsFiles = dcsFiles;
            _expandedChallenges = expandedChallenges;
            _shell = shell;
        }

        public const string Challenge = Program.DefaultChallenge;

        [Required]
        public int Stage { get; set; }

        public override bool Execute()
        {
            var challengeDir = _dcsFiles.ChallengesSource.Challenges[Challenge];
            StageSourceDirectory sourceDir = null;
            var stage = new Stage(Stage);
            sourceDir = stage.Number == 0
                ? challengeDir.Stage("tip")
                : _expandedChallenges.ExpandStages(Challenge, new Stage(Stage - 1));
            var targetDir = challengeDir.Stage(stage);

            _log.InfoFormat("{0} -> {1}", sourceDir, targetDir);
            targetDir.DeleteContents();
            targetDir.Create();
            sourceDir.CopyContentsInto(targetDir);
            return true;
        }
    }
}