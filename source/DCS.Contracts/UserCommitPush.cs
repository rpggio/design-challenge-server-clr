using System;
using System.ComponentModel.DataAnnotations;

namespace DCS.Contracts
{
    public class UserCommitPush
    {
        [Required]
        public string CommitId { get; set; }

        [Required]
        public string Repository { get; set; }
        
        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime CommittedAt { get; set; }

        public string Message { get; set; }
    }
}