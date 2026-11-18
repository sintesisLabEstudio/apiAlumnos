using AlumnosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Data;

public class AlumnosContext : DbContext
{
    public AlumnosContext(DbContextOptions<AlumnosContext> options) : base(options) { }

    public DbSet<Alumno> Alumnos => Set<Alumno>();

    public DbSet<Asignatura> Asignaturas => Set<Asignatura>();

    public DbSet<Nota> Notas => Set<Nota>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Alumno>(e =>
        {
            e.ToTable("ALUMNO");
            e.HasKey(x => x.CodAlumno);
            e.Property(x => x.CodAlumno)
                .HasColumnName("COD_ALUMNO")
                .ValueGeneratedNever();
            e.Property(x => x.Nombres)
                .HasColumnName("NOMBRES")
                .HasColumnType("varchar(50)");
            e.Property(x => x.Apellidos)
                .HasColumnName("APELLIDOS")
                .HasColumnType("varchar(50)");
        });

        mb.Entity<Asignatura>(e =>
        {
            e.ToTable("ASIGNATURA");

            e.HasKey(x => x.IdAsignatura);

            e.Property(x => x.IdAsignatura)
                .HasColumnName("ID_ASIGNATURA");

            e.Property(x => x.NomAsignatura)
                .HasColumnName("NOM_ASIGNATURA")
                .HasColumnType("varchar(50)");
        });

        mb.Entity<Nota>(e =>
        {
            e.ToTable("NOTAS");

            e.HasKey(x => x.IdNota);

            e.Property(x => x.IdNota)
                .HasColumnName("ID_NOTA");

            e.Property(x => x.Valor)
                .HasColumnName("NOTA")
                .HasColumnType("decimal(2,1)");

            e.Property(x => x.NNota)
                .HasColumnName("N_NOTA");

            e.Property(x => x.CodAlumno)
                .HasColumnName("COD_ALUMNO");

            e.Property(x => x.IdAsignatura)
                .HasColumnName("ID_ASIGNATURA");
        });
    }
}
