using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Bootstrap;
using DCS.ServerRuntime.Services;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;
using Newtonsoft.Json;

namespace DCS.Console.Commands
{
    [Description("Test a range of stages")]
    public class TestStagesCommand : ConsoleCommandBase
    {
        private readonly ExpandedChallenges _expandedChallenges;
        private readonly AssessmentService _assessmentService;
        private readonly DcsFilesystem _dcsFilesystem;
        private readonly ILog _log;
        private readonly KillOrphansOperation _killOrphansOperation;

        public TestStagesCommand(DcsFilesystem dcsFilesystem, ExpandedChallenges expandedChallenges,
            AssessmentService assessmentService, ILog log, KillOrphansOperation killOrphansOperation)
        {
            _expandedChallenges = expandedChallenges;
            _assessmentService = assessmentService;
            _dcsFilesystem = dcsFilesystem;
            _log = log;
            _killOrphansOperation = killOrphansOperation;

            Cleanup = true;
        }

        public const string Challenge = Program.DefaultChallenge;

        public string Solution { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        public bool Cleanup { get; set; }

        public override bool Execute()
        {
            if (!To.HasValue && From.HasValue)
            {
                To = From;
            }
            if (!From.HasValue && To.HasValue)
            {
                From = To;
            }

            if (From > To)
            {
                throw new Exception("Invalid args");
            }

            _log.Debug("Cleaning up orphan processes");
            _killOrphansOperation.Execute();

            var challengeDir = _dcsFilesystem.ChallengesSource.Challenges[Challenge];
            if (Solution == null)
            {
                Solution = challengeDir.Stage(new Stage(0)).Solutions.Single().Key;
            }
            _log.InfoFormat("Using challenge {0}", Challenge);
            _log.InfoFormat("Using solution {0}", Solution);

            var expandedChallenge = _expandedChallenges.GetExpandedChallenge(Challenge);
            bool passed = true;

            for (var stage = new Stage(From.Value); stage.Number <= To.Value; stage += 1)
            {
                var expandedStage = expandedChallenge.Stage(stage);

                var stageBuildDir = _dcsFilesystem.CreateBuildDir("{0}-{1}".FormatFrom(Challenge, stage));

                _log.InfoFormat("Copying content into {0}", stageBuildDir);
                expandedStage.CopySolutionIntoBuildDir(Solution, stageBuildDir);

                _log.InfoFormat("Testing {0}", stageBuildDir);
                var assessment = _assessmentService.Assess(Challenge, stage, stageBuildDir);
                string output = assessment.BuildOutput;
                assessment.BuildOutput = output.Left(200);
                if (assessment.Outcome != AssessmentOutcome.Success)
                {
                    passed = false;
                    _log.Warn(JsonConvert.SerializeObject(assessment));
                    _log.Debug(output);

                    System.Console.WriteLine(output);
                }
                else
                {
                    _log.Info(JsonConvert.SerializeObject(assessment));
                    _log.Debug(output);
                }

                if (Cleanup)
                {
                    Thread.Sleep(50);
                    if (!FuncUtil.TryExecute(stageBuildDir.Delete))
                    {
                        _log.WarnFormat("Failed to clean up build dir {0}", stageBuildDir);
                    }
                }
            }

            _log.Info(passed ? "Stages passed" : "Stages failed");

            return passed;
        }
    }
}