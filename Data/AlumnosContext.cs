using AlumnosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Data;

// ===================================================================
// CONTEXTO DE DATOS: AlumnosContext
// ===================================================================
// Este es el "intermediario" entre nuestra aplicación y la base de datos
// Entity Framework usa esta clase para leer y escribir en SQL Server
// Es como un "intérprete" que traduce nuestro código C# a comandos SQL

public class AlumnosContext : DbContext
{
    // Constructor
    // DbContextOptions contiene la configuración de conexión
    // Se pasa automáticamente desde Program.cs
    public AlumnosContext(DbContextOptions<AlumnosContext> options) : base(options) { }

    // DbSet: Tablas de la base de datos
    // Cada DbSet representa una tabla en la BD
    // Set<Alumno>() obtiene la colección de alumnos de la BD

    /// Tabla ALUMNOS - Contiene todos los alumnos registrados
    /// Accedemos así: _db.Alumnos.ToListAsync() para listar, _db.Alumnos.Add() para agregar
    public DbSet<Alumno> Alumnos => Set<Alumno>();

    /// Tabla ASIGNATURAS - Contiene todas las asignaturas disponibles
    /// Ejemplo: Matemática, Lenguaje, Historia, Inglés, etc.
    public DbSet<Asignatura> Asignaturas => Set<Asignatura>();

    /// Tabla NOTAS - Contiene las calificaciones de cada alumno por asignatura
    /// Relaciona: Qué alumno (CodAlumno) tuvo qué nota (Valor) en qué asignatura (IdAsignatura)
    public DbSet<Nota> Notas => Set<Nota>();

    // OnModelCreating: Configuración del mapeo ORM
    // Aquí definimos la relación entre nuestros objetos C# y las tablas SQL
    // Es como decir: "Este campo C# se llama NOMBRES en la tabla SQL"
    // IMPORTANTE: Los nombres de tabla DEBEN coincidir exactamente con los de la BD

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Configuración de tabla ALUMNOS
        mb.Entity<Alumno>(e =>
        {
            // Especificar el nombre exacto de la tabla en SQL Server
            e.ToTable("ALUMNOS");
            // Definir la clave primaria (no puede repetirse)
            e.HasKey(x => x.CodAlumno);
            // Mapear cada propiedad de C# a su columna en SQL
            // HasColumnName("COD_ALUMNO") = la columna en BD se llama "COD_ALUMNO"
            // ValueGeneratedNever() = no se genera automáticamente (la asignamos nosotros)
            e.Property(x => x.CodAlumno)
                .HasColumnName("COD_ALUMNO")
                .ValueGeneratedNever();
            // Mapear el campo Nombres a su columna NOMBRES en la BD
            e.Property(x => x.Nombres)
                .HasColumnName("NOMBRES");
            // Mapear el campo Apellidos a su columna APELLIDOS en la BD
            e.Property(x => x.Apellidos)
                .HasColumnName("APELLIDOS");
        });

        // Configuración de tabla ASIGNATURAS
        mb.Entity<Asignatura>(e =>
        {
            e.ToTable("ASIGNATURAS");

            // IdAsignatura es la clave primaria
            // Sin ValueGeneratedNever() porque SQL Server la genera automáticamente
            e.HasKey(x => x.IdAsignatura);

            e.Property(x => x.IdAsignatura)
                .HasColumnName("ID_ASIGNATURA");

            e.Property(x => x.NomAsignatura)
                .HasColumnName("NOM_ASIGNATURA");
        });

        // Configuración de tabla NOTAS
        mb.Entity<Nota>(e =>
        {
            e.ToTable("NOTAS");

            // IdNota es la clave primaria de la tabla de notas
            e.HasKey(x => x.IdNota);

            e.Property(x => x.IdNota)
                .HasColumnName("ID_NOTA");

            // Valor: la calificación (0 a 10, con un decimal)
            // HasColumnType("decimal(2,1)") = máximo 2 dígitos, 1 decimal (Ej: 9.5)
            e.Property(x => x.Valor)
                .HasColumnName("NOTA")
                .HasColumnType("decimal(2,1)");

            // Número de nota para el alumno en esa asignatura (1era, 2da, 3era nota)
            e.Property(x => x.NNota)
                .HasColumnName("N_NOTA");

            // Relación con Alumno: qué alumno tiene esta nota
            e.Property(x => x.CodAlumno)
                .HasColumnName("COD_ALUMNO");

            // Relación con Asignatura: en qué asignatura es esta nota
            e.Property(x => x.IdAsignatura)
                .HasColumnName("ID_ASIGNATURA");
        });
    }
}
