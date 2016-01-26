using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DCS.Core;

namespace DCS.Contracts.Entities
{
    [ServiceStack.DataAnnotations.Alias("User")]
    public class UserEntity : IEntity<Guid>, IScmUser
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Description("Email address of user")]
        [Required]
        [StringLength(500)]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        public bool IsTestUser { get; set; }

        public List<string> Claims { get; set; }

        [ServiceStack.DataAnnotations.Reference]
        public List<UserChallengeEntity> Challenges { get; set; }

        [ServiceStack.DataAnnotations.Reference]
        public List<RepositoryEntity> Repositories { get; set; } 

        public UserEntity()
        {
            Claims = new List<string>();
            Challenges = new List<UserChallengeEntity>();
            Repositories = new List<RepositoryEntity>();
        }

        public UserChallengeEntity AddChallenge(string challengeName, Guid repositoryId)
        {
            var challenge = new UserChallengeEntity
                    {
                        Name = challengeName,
                        StageNumber = Stage.First.Number,
                        RepositoryId = repositoryId
                    };
            Challenges.Add(challenge);
            return challenge;
        }

        public UserChallengeEntity GetChallenge(string challenge)
        {
            return Challenges.FirstOrDefault(c => c.Name.EqualsIgnoreCase(challenge));
        }

        public UserChallengeEntity GetChallengeForRepo(Guid repoId)
        {
            return Challenges.FirstOrDefault(c => c.RepositoryId  == repoId);
        }
    }
}