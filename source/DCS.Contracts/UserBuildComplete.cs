using System;

namespace DCS.Contracts
{
    public class UserBuildComplete : IUserAware, IChallengeAction
    {
        public bool Passed { get; set; }
        public string CommitId { get; set; }
        public Guid UserId { get; set; }
        public string ChallengeName { get; set; }
        public string Repository { get; set; }
        public string Output { get; set; }
        public string BuildDir { get; set; }
        public string FromStage { get; set; }
    }
}