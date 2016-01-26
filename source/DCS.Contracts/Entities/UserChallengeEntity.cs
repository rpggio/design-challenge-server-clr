using System;
using System.ComponentModel.DataAnnotations;
using DCS.Core;

namespace DCS.Contracts.Entities
{
    [ServiceStack.DataAnnotations.Alias("UserChallenge")]
    public class UserChallengeEntity
    {
        [ServiceStack.DataAnnotations.PrimaryKey]
        [ServiceStack.DataAnnotations.AutoIncrement]
        public int Id { get; set; }

        [ServiceStack.DataAnnotations.ForeignKey(typeof(UserEntity))]
        public Guid UserId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        
        public int StageNumber { get; set; }

        public Guid RepositoryId { get; set; }

        [ServiceStack.DataAnnotations.Ignore] 
        public Stage Stage
        {
            get { return new Stage(StageNumber); }
        }

        public override string ToString()
        {
            return "{0}-{1}.{2}".FormatFrom(Name, RepositoryId.ToString("N"), Stage.Number);
        }
    }
}