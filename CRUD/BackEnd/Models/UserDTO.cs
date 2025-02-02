using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("userDto")]
    public class UserDTO
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("age")]
        public int Age { get; set; }
    }
}
