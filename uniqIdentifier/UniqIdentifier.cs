using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.uniqIdentifier;
[Table("temporary_uniqid")]
public class UniqIdentifier
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("uniqid")]
    public string Uniqid { get; set; }
    
    [Column("date_debut")]
    [Required]
    public DateTime StartDate { get; set; }

    [Column("date_fin")]
    [Required]
    public DateTime EndDate { get; set; }

    [Column("id_user")]
    [Required]
    public int UserId { get; set; }
}