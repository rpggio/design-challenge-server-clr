using System.ComponentModel.DataAnnotations;

namespace DCS.Contracts
{
    public class CreateUser //: IValidatableObject
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsTestUser { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    return new Validator().Validate(this).ToValidationResult();
        //}

        //private class Validator : AbstractValidator<CreateUser>
        //{
        //    public Validator()
        //    {
        //        RuleFor(o => o.User)
        //            .NotNull()
        //            .SetValidator(new UserValidator());
        //    }
        //}

        //private class UserValidator : AbstractValidator<UserEntity>
        //{
        //    public UserValidator()
        //    {
        //        RuleFor(o => o.Username)
        //            .NotEmpty();

        //        RuleFor(o => o.Password)
        //            .NotEmpty();

        //        RuleFor(o => o.Email)
        //            .NotEmpty()
        //            .Must(e => e.Contains("@"));
        //    }
        //}
    }
}