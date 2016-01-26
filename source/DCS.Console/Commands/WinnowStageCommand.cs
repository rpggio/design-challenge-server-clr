using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;

namespace DCS.Console.Commands
{
    [Description("Build stage contents from tip relative to prior stage")]
    public class WinnowStageCommand : ConsoleCommandBase
    {
        private readonly ILog _log;
        private readonly DcsFilesystem _dcsFiles;
        private readonly ExpandedChallenges _expandedChallenges;
        private readonly Shell _shell;

        public WinnowStageCommand(ILog log, DcsFilesystem dcsFiles, ExpandedChallenges expandedChallenges, Shell shell)
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
            if (Stage == 0)
            {
                throw new Exception("Cannot winnow stage 0");
            }

            var stage = new Stage(Stage);
            var challengeDir = _dcsFiles.ChallengesSource.Challenges[Challenge];
            var stageDir = challengeDir.Stage(stage);
            _log.InfoFormat("Winnowing stage dir {0}", stageDir);

            var priorStageExpanded = _expandedChallenges.ExpandStages(Challenge, stage - 1);
            foreach (var file in stageDir.Recurse().OfType<IFile>())
            {
                string relativePath = file.PathRelativeFrom(stageDir);
                var priorFile = priorStageExpanded.File(relativePath);
                if (priorFile.ContentsAreEqualTo(file))
                {
                    _log.DebugFormat("{0} X", file);
                    file.Delete();
                }

                // alternate diff method using Git diff
                //var priorInfo = priorFile.Info();
                //var fileInfo = file.Info();
                //if (priorInfo.Exists
                //    && priorInfo.Length == fileInfo.Length
                //    || _shell.RunBackground("diff", "{0} {1}".FormatFrom(priorFile, file)) == 0)
            }

            foreach (var dir in stageDir.Recurse().OfType<IDirectory>().Reverse())
            {
                if (dir.IsEmpty())
                {
                    dir.Delete();
                }
            }

            return true;
        }
    }
}