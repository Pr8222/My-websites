using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("user")]
    public class User
    {
        [Required]
        [Column("id"), Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [Column("username")]
        public string UserName { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be 6-100 characters!")]
        [Column("password")]
        public string Password { get; set; }
        [Required]
        [Column("age")]
        public int Age { get; set; }
        [Column("role")]
        public string Role {  get; set; }
    }
}
