namespace AlumnosApi.Models;

// ===================================================================
// MODELO: Alumno
// ===================================================================
// Este es la "forma" o "estructura" de datos para un alumno
// Es como el molde de una galleta - define qué información tiene cada alumno
// Entity Framework usará esta clase para crear y leer datos en la BD

public class Alumno
{
    // 🆔 Identificador único del alumno (clave primaria)
    // Cada alumno tiene un código diferente
    // { get; set; } permite leer y cambiar el valor
    public int CodAlumno { get; set; }

    // 📝 Nombre(s) del alumno
    // El "?" significa que puede ser vacío/nulo (opcional)
    public string? Nombres { get; set; }

    // 📝 Apellidos del alumno
    // También puede ser vacío/nulo
    public string? Apellidos { get; set; }
}
