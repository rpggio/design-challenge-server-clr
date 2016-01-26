using System;
using System.ComponentModel.DataAnnotations;
using DCS.Core;

namespace DCS.Contracts.Entities
{
    [ServiceStack.DataAnnotations.Alias("Repository")]
    public class RepositoryEntity : IEntity<Guid>
    {
        [ServiceStack.DataAnnotations.PrimaryKey]
        public Guid Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Challenge { get; set; }

        public DateTime? LockedAt { get; set; }

        [ServiceStack.DataAnnotations.ForeignKey(typeof(UserEntity))]
        public Guid UserId { get; set; }

        [ServiceStack.DataAnnotations.Reference]
        public UserEntity User { get; set; }

        public bool Matches(string name)
        {
            return Name.EqualsIgnoreCase(name.Replace(".git", ""));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}