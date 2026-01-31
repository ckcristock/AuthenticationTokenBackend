using Microsoft.EntityFrameworkCore;
using AuthenticationTokenBackend.Models;

namespace AuthenticationTokenBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Usuarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación User-Tarea
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único en el nombre de usuario
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Usuario)
                .IsUnique();

            // Valores por defecto
            modelBuilder.Entity<User>()
                .Property(u => u.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Tarea>()
                .Property(t => t.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
