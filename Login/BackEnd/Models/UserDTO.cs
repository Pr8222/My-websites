using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("userDTO")]
    public class UserDTO : IValidatableObject
    {

        [Column("username")]
        public string UserName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("age")]
        public int Age { get; set; }

        public IEnumerable<ValidationResult> Validate (ValidationContext validationContext)
        {
            // Define invalid Swagger example values
            var invalidSwaggerValues = new HashSet<string>
        {
            "string", "example@example.com", "password123"
        };

            if (invalidSwaggerValues.Contains(UserName) ||
                invalidSwaggerValues.Contains(Email) ||
                invalidSwaggerValues.Contains(Password))
            {
                yield return new ValidationResult("Invalid default values detected.", new[] { nameof(UserName), nameof(Email), nameof(Password)});
            }
        }
    }
}
