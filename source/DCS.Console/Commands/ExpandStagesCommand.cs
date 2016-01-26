using System.ComponentModel;
using DCS.ServerRuntime.Services;
using log4net;

namespace DCS.Console.Commands
{
    [Description("Expand stage contents for challenge")]
    public class ExpandStagesCommand : ConsoleCommandBase
    {
        private readonly ILog _log;
        private readonly ExpandedChallenges _expandedChallenges;

        public const string Challenge = Program.DefaultChallenge;

        public ExpandStagesCommand(ILog log, ExpandedChallenges expandedChallenges)
        {
            _log = log;
            _expandedChallenges = expandedChallenges;
        }

        public override bool Execute()
        {
            _expandedChallenges.ExpandStages(Challenge);
            return true;
        }
    }
}