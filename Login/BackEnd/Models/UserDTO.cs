﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("userDTO")]
    public class UserDTO
    {

        [Column("username")]
        public string UserName { get; set; }


        [Column("email")]
        public string Email { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("password")]
        public string Password { get; set; }

    }
}
