using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationTokenBackend.Models
{
    [Table("tareas")]
    public class Tarea
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("titulo")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Column("descripcion")]
        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Required]
        [Column("completada")]
        public bool Completada { get; set; } = false;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }

        // Foreign Key
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual User? Usuario { get; set; }
    }
}
