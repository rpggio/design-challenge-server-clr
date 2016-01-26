using System;
using System.ComponentModel.DataAnnotations;

namespace DCS.Contracts.Entities
{
    [ServiceStack.DataAnnotations.Alias("Contact")]
    public class ContactEntity : IEntity<int>
    {
        [Required]
        [ServiceStack.DataAnnotations.PrimaryKey]
        [ServiceStack.DataAnnotations.AutoIncrement]
        public int Id { get; set; }

        [EmailAddress]
        [Required]
        [StringLength(500)]
        public string Email { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}
