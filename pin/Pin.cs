using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.pin;

[Table("Pin")]
public class Pin
{
    [Key]
    [Column("id_pin")]
    public int IdPin  { get; set; }
    
    [Column("id_user")]
    public int IdUser { get; set; }
    
    [Column("pin_number")]
    public int PinNumber { get; set; }
    
    [Column("date_debut")]
    public DateTime DateDebut { get; set; }
    
    [Column("date_fin")]
    public DateTime DateFin { get; set; }
}