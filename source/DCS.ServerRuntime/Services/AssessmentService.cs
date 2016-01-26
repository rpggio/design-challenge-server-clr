using System;
using System.Diagnostics;
using DCS.Contracts;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Entities;
using DCS.ServerRuntime.Framework;
using DCS.ServerRuntime.Services.FileSystem;
using log4net;

namespace DCS.ServerRuntime.Services
{
    [RegisterComponent]
    public class AssessmentService
    {
        private readonly ILog _log;
        private readonly ExpandedChallenges _expandedChallenges;
        private readonly SolutionRunner _runner;

        public AssessmentService(ILog log, ExpandedChallenges expandedChallenges, SolutionRunner runner)
        {
            _log = log;
            _expandedChallenges = expandedChallenges;
            _runner = runner;
        }

        public AssessmentResult Assess(string challenge, Stage stage, StageBuildDirectory stageBuildDir)
        {
            // get solution settings

            var settingsFile = stageBuildDir.SolutionConfigFile;
            if (!settingsFile.Exists())
            {
                return new AssessmentResult(AssessmentOutcome.InvalidContent)
                {
                    Message = "Could not find solution-config.json in the solution root",
                };
            }
            var solutionSettings = settingsFile.Read();
            if (string.IsNullOrEmpty(solutionSettings.Run))
            {
                return new AssessmentResult(AssessmentOutcome.InvalidContent)
                {
                    Message = "'start' param not provided in solution-config.json",
                };
            }

            // build solution

            if (solutionSettings.MsBuild != null)
            {
                string buildSolution = stageBuildDir.File(solutionSettings.MsBuild).Path;
                _log.DebugFormat("Building {0}", buildSolution);
                var buildProcessStart = new ProcessStartInfo("msbuild", buildSolution);

                using (var buildRun = ProcessRun.Start(buildProcessStart))
                {
                    if (!buildRun.Task.Wait(TimeSpan.FromSeconds(15)))
                    {
                        buildRun.Kill();
                        return new AssessmentResult(AssessmentOutcome.BuildFailure)
                        {
                            Message = "Build timed out",
                            BuildOutput = buildRun.Task.Result.StandardOutput,
                        };
                    }

                    if (buildRun.Task.Result.ExitCode != 0)
                    {
                        return new AssessmentResult(AssessmentOutcome.BuildFailure)
                        {
                            Message = "Build failed",
                            BuildOutput = buildRun.Task.Result.OutputSummary,
                        };
                    }
                }
            }

            // get start file

            var startFile = stageBuildDir.File(solutionSettings.Run);
            _log.DebugFormat("Starting {0}", startFile);
            if (!startFile.Exists())
            {
                return new AssessmentResult(AssessmentOutcome.BuildFailure)
                {
                    Message = string.Format("Could not find executable {0} specified in solution-config.json",
                        solutionSettings.Run),
                };
            }

            // update features

            var stageExpandedDir = _expandedChallenges
                .GetExpandedChallenge(challenge)
                .Stage(stage);
            if (!stageBuildDir.PathEquals(stageExpandedDir))
            {
                var buildFeaturesDir = stageBuildDir.Directory("features");
                buildFeaturesDir.DeleteContents();
                stageExpandedDir.Directory("features").CopyContentsInto(buildFeaturesDir);
            }

            // run assessment

            _log.DebugFormat("Starting solution assessment {0} {1}", startFile, stageBuildDir);
            var result = _runner.Run(startFile, stageBuildDir);
            return result;
        }
    }
}
