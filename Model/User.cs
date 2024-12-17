using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.Model;

[Table("user")]
public class User {
    [Key]
    [Column("id_user")]
    public int IdUser { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("password")]
    public string Password { get; set; }


}