using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace DCS.Core.Validation
{
    /// <summary>
    /// Extension to the Fluent Validation Types
    /// </summary>
    public static class ValidationUtil
    {
        /// <summary>
        /// Converts the Fluent Validation result to the type the both mvc and ef expect
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> ToValidationResult(
            this FluentValidation.Results.ValidationResult validationResult)
        {
            return validationResult.Errors.Select(item => 
                new ValidationResult(item.ErrorMessage, new List<string> { item.PropertyName }));
        }

        /// <summary>
        /// Run validator against object.
        /// </summary>
        public static bool TryValidate(object obj, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(obj, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                obj, context, results, true
            );
        }

        /// <summary>
        /// Run validator against object.
        /// </summary>
        public static bool IsValid(object obj)
        {
            ICollection<ValidationResult> results;
            return TryValidate(obj, out results);
        }

        /// <summary>
        /// Throw exception if object is not valid.
        /// </summary>
        public static void AssertIsValid(object obj)
        {
            ICollection<ValidationResult> results;
            if (!TryValidate(obj, out results))
            {
                throw new ValidationException(results.Select(r => r.ErrorMessage).JoinString(", "));
            }
        }
    }
}