using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.userValidation;

[Table("user_validation")]
public class UserValidation {
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uid")] 
    public string? Uid { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("password")]
    public string Password { get; set; }
    
}