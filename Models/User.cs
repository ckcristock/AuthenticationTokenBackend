using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationTokenBackend.Models
{
    [Table("usuarios")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("usuario")]
        [StringLength(50)]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [Column("contrasena")]
        [StringLength(255)]
        public string Contrasena { get; set; } = string.Empty;

        [Column("token")]
        [StringLength(500)]
        public string? Token { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_ultimo_login")]
        public DateTime? FechaUltimoLogin { get; set; }

        // Relación con tareas
        public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}
