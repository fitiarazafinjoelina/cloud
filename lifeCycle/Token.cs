using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloud.lifeCycle
{
    [Table("token")]
    public class Token
    {
        [Key]
        [Column("id_token")]
        public int Id { get; set; }

        [Column("token")]
        [Required]
        [MaxLength(255)]
        public string Value { get; set; }

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
}
