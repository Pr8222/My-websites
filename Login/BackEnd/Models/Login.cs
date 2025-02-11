using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("user")]
    public class Login
    {
        [Column("username")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
