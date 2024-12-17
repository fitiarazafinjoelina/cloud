
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.Model;

[Table("token")]
public class Token {
    [Key]
    [Column("id_token")]
    public int IdToken { get; set; }

    [Column("token")]
    public string Valeur { get; set; }

    [Column("date_debut")]
    public DateTime DateDebut { get; set; }

    [Column("date_fin")]
    public DateTime DateFin { get; set; }

    [Column("id_user")]
    public User idUser { get; set; }
}