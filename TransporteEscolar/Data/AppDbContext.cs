using Microsoft.EntityFrameworkCore;
using TransporteEscolar.Models;

namespace TransporteEscolar.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Autobus> Autobuses { get; set; }
        public DbSet<BitacoraAcceso> BitacoraAccesos { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<RegistroAbordaje> RegistrosAbordaje { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Rol ─────────────────────────────────────────────
            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(r => r.RolId);
                e.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
                e.Property(r => r.Descripcion).HasMaxLength(200);
                e.HasIndex(r => r.Nombre).IsUnique();
            });

            // ── Autobús ──────────────────────────────────────────
            modelBuilder.Entity<Autobus>(e =>
            {
                e.ToTable("Autobuses");
                e.HasKey(a => a.AutobusId);
                e.Property(a => a.Ficha).IsRequired().HasMaxLength(20);
                e.Property(a => a.Placa).IsRequired().HasMaxLength(20);
                e.HasIndex(a => a.Ficha).IsUnique();
                e.HasIndex(a => a.Placa).IsUnique();
            });

            // ── Usuario ──────────────────────────────────────────
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(u => u.UsuarioId);
                e.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(100);
                e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
                e.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(200);
                e.Property(u => u.Email).HasMaxLength(200);
                e.HasIndex(u => u.NombreUsuario).IsUnique();

                // Relación con Rol
                e.HasOne(u => u.Rol)
                 .WithMany(r => r.Usuarios)
                 .HasForeignKey(u => u.RolId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Relación con Autobús (nullable — solo Azafata)
                e.HasOne(u => u.Autobus)
                 .WithMany(a => a.Usuarios)
                 .HasForeignKey(u => u.AutobusId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Bitácora ─────────────────────────────────────────
            modelBuilder.Entity<BitacoraAcceso>(e =>
            {
                e.ToTable("BitacoraAccesos");
                e.HasKey(b => b.BitacoraId);
                e.Property(b => b.NombreUsuario).IsRequired().HasMaxLength(100);
                e.Property(b => b.DireccionIP).HasMaxLength(50);
                e.Property(b => b.UserAgent).HasMaxLength(500);
                e.Property(b => b.Detalle).HasMaxLength(500);

                e.HasOne(b => b.Usuario)
                 .WithMany()
                 .HasForeignKey(b => b.UsuarioId)
                 .IsRequired(false)
                 .OnDelete(DeleteBehavior.SetNull);
            });
            // ── Estudiante ─────────────────────────
            modelBuilder.Entity<Estudiante>(e =>
            {
                e.ToTable("Estudiantes");
                e.HasKey(e => e.IdEstudiante);
                e.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                e.Property(e => e.Apellido1).HasMaxLength(100);
                e.Property(e => e.Apellido2).HasMaxLength(100);
                e.Property(e => e.CodigoQR).HasMaxLength(100);
            });

            // ── Registro Abordaje ──────────────────
            modelBuilder.Entity<RegistroAbordaje>(e =>
            {
                e.ToTable("RegistroAbordaje");
                e.HasKey(r => r.IdRegistro);

                e.HasOne(r => r.Estudiante)
                 .WithMany()
                 .HasForeignKey(r => r.IdEstudiante);
            });
        }
    }
}