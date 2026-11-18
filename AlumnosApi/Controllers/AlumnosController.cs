using AlumnosApi.Data;  // Importa el contexto de BD
using AlumnosApi.Models;  // Importa los modelos (Alumno, etc.)
using Microsoft.AspNetCore.Mvc;  // Importa controlador base y atributos
using Microsoft.EntityFrameworkCore;  // Importa funciones async de EF Core

namespace AlumnosApi.Controllers;

[ApiController]
[Route("api/alumnos")]
public class AlumnosController : ControllerBase
{
    private readonly AlumnosContext _db;
    public AlumnosController(AlumnosContext db) => _db = db;

    /// <summary>LISTAR todos los alumnos ordenados por codigo.</summary>
    [HttpGet("listar")]
    public async Task<IActionResult> ListarAlumnos()
    {
        var lista = await _db.Alumnos.OrderBy(a => a.CodAlumno).ToListAsync();
        return Ok(lista);
    }

    /// <summary>GRABAR un alumno nuevo (codigo, nombres y apellidos).</summary>
    [HttpPost("grabar")]
    public async Task<IActionResult> GrabarAlumno(Alumno alumno)
    {
        if (alumno.CodAlumno <= 0)
            return BadRequest("El código de alumno debe ser mayor a 0.");

        if (string.IsNullOrWhiteSpace(alumno.Nombres) || string.IsNullOrWhiteSpace(alumno.Apellidos))
            return BadRequest("Nombres y apellidos son obligatorios.");

        if (await _db.Alumnos.AnyAsync(a => a.CodAlumno == alumno.CodAlumno))
            return Conflict($"Ya existe un alumno con código {alumno.CodAlumno}.");

        alumno.Nombres = alumno.Nombres.Trim();
        alumno.Apellidos = alumno.Apellidos.Trim();

        _db.Alumnos.Add(alumno);
        await _db.SaveChangesAsync();
        return Ok(alumno);
    }

    /// <summary>EDITAR los nombres y apellidos de un alumno existente.</summary>
    [HttpPut("editar")]
    public async Task<IActionResult> EditarAlumno(Alumno alumno)
    {
        if (string.IsNullOrWhiteSpace(alumno.Nombres) || string.IsNullOrWhiteSpace(alumno.Apellidos))
            return BadRequest("Nombres y apellidos son obligatorios.");
        var existente = await _db.Alumnos.FirstOrDefaultAsync(a => a.CodAlumno == alumno.CodAlumno);
        if (existente == null)
            return NotFound($"No existe un alumno con código {alumno.CodAlumno}.");
        existente.Nombres = alumno.Nombres.Trim();
        existente.Apellidos = alumno.Apellidos.Trim();
        await _db.SaveChangesAsync();
        return Ok(existente);
    }

    /// <summary>BORRAR un alumno y todas sus notas por codigo de alumno.</summary>
    [HttpDelete("borrar/{codAlumno:int}")]
    public async Task<IActionResult> BorrarAlumno(int codAlumno)
    {
        var existente = await _db.Alumnos.FirstOrDefaultAsync(a => a.CodAlumno == codAlumno);
        if (existente == null)
            return NotFound($"No existe un alumno con código {codAlumno}.");

        var notas = await _db.Notas.Where(n => n.CodAlumno == codAlumno).ToListAsync();
        if (notas.Count > 0)
        {
            _db.Notas.RemoveRange(notas);
            await _db.SaveChangesAsync();
        }
        _db.Alumnos.Remove(existente);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Alumno borrado.", codAlumno });
    }
}
