using System;
using System.ComponentModel.DataAnnotations;

namespace DCS.Contracts.Entities
{
    [ServiceStack.DataAnnotations.Alias("Commit")]
    public class CommitEntity : IEntity<string>
    {
        [ServiceStack.DataAnnotations.PrimaryKey]
        [StringLength(40)]
        [Required]
        public string Id { get; set; }

        [Required]
        public DateTime CommittedAt { get; set; }

        [Required]
        [ServiceStack.DataAnnotations.ForeignKey(typeof(UserEntity))]
        public Guid? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [StringLength(500)]
        public string CommitMessage { get; set; }

        [Required]
        [ServiceStack.DataAnnotations.ForeignKey(typeof(RepositoryEntity))]
        public Guid? RepositoryId { get; set; }

        [Required]
        [StringLength(200)]
        public string RepositoryName { get; set; }

        [Required]
        [StringLength(100)]
        public string Challenge { get; set; }
        
        public int? CurrentStageNumber { get; set; }
        
        public DateTime? ResultsUpdatedAt { get; set; }

        [StringLength(50)]
        public AssessmentOutcome? Outcome { get; set; }
        
        [StringLength(1000)] 
        public string OutcomeDetail { get; set; }

        [StringLength(int.MaxValue)] 
        public string BuildLog { get; set; }

        [StringLength(10)]
        public TestOutputFormat TestOutputFormat { get; set; }
        
        [StringLength(int.MaxValue)] 
        public string TestOutput { get; set; }

        [ServiceStack.DataAnnotations.IgnoreAttribute]
        public string ShortId
        {
            get { return Id == null ? null : Id.Substring(0, 7); }
        }
    }
}