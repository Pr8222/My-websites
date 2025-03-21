﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("user")]
    public class User : IValidatableObject
    {

        [Column("user_id"), Key]
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
        public Role Role { get; set; }
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

        public string Name { get; set; }
    }
}
