using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("user")]
    public class User : IValidatableObject
    {
        [Key]
        [Column("user_id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [StringLength(50, ErrorMessage = "The username is too long!")]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Invalid username Format!")]
        [Column("username")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format!")]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be 6-100 characters!")]
        [Column("password")]
        public string Password { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }
        public ICollection<Role> Roles { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Define invalid Swagger example values
            var invalidSwaggerValues = new HashSet<string>
        {
            "username", "example@example.com", "password123"
        };

            if (invalidSwaggerValues.Contains(UserName) ||
                invalidSwaggerValues.Contains(Email) ||
                invalidSwaggerValues.Contains(Password))
            {
                yield return new ValidationResult("Invalid default values detected.", new[] { nameof(UserName), nameof(Email), nameof(Password) });
            }
        }
    }
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<RoleKeys> RoleKeys { get; set; }
    }

    public class RoleUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class RoleKeys
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int KeyId { get; set; }
        public Key Key { get; set; }
    }

    public class Key
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string FriendlyKeyName { get; set; }

        public ICollection<RoleKeys> RoleKeys { get; set; }
        public ICollection<UserExtraKeys> UserKeys { get; set; }
    }

    public class UserExtraKeys
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int KeyId { get; set; }
        public Key Key { get; set; }

        public string? KeyValue { get; set; } // This is the value of the key for the user
        public DateTime? ExpirationDate { get; set; } // This is the expiration date of the key for the user
    }


}
