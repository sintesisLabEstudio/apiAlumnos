using AlumnosApi.Data;
using AlumnosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Controllers;

[ApiController]
[Route("api/asignaturas")]
public class AsignaturasController : ControllerBase
{
    private readonly AlumnosContext _db;
    public AsignaturasController(AlumnosContext db) => _db = db;

    /// <summary>LISTAR todas las asignaturas ordenadas por id.</summary>
    [HttpGet("listar")]
    public async Task<IActionResult> ListarAsignaturas()
    {
        var lista = await _db.Asignaturas.OrderBy(a => a.IdAsignatura).ToListAsync();
        return Ok(lista);
    }

    /// <summary>GRABAR una asignatura nueva (id autogenerado).</summary>
    [HttpPost("grabar")]
    public async Task<IActionResult> GrabarAsignatura(Asignatura asignatura)
    {
        if (string.IsNullOrWhiteSpace(asignatura.NomAsignatura))
            return BadRequest("El nombre de la asignatura es obligatorio.");

        var nombre = asignatura.NomAsignatura.Trim();
        if (await _db.Asignaturas.AnyAsync(a => a.NomAsignatura == nombre))
            return Conflict("Ya existe una asignatura con ese nombre.");

        asignatura.IdAsignatura = 0;
        asignatura.NomAsignatura = nombre;
        _db.Asignaturas.Add(asignatura);
        await _db.SaveChangesAsync();
        return Ok(asignatura);
    }

    /// <summary>EDITAR el nombre de una asignatura existente.</summary>
    [HttpPut("editar")]
    public async Task<IActionResult> EditarAsignatura(Asignatura asignatura)
    {
        if (string.IsNullOrWhiteSpace(asignatura.NomAsignatura))
            return BadRequest("El nombre de la asignatura es obligatorio.");
        var nombre = asignatura.NomAsignatura.Trim();
        var existente = await _db.Asignaturas.FirstOrDefaultAsync(a => a.IdAsignatura == asignatura.IdAsignatura);
        if (existente == null)
            return NotFound("La asignatura no existe.");
        if (await _db.Asignaturas.AnyAsync(a => a.NomAsignatura == nombre && a.IdAsignatura != asignatura.IdAsignatura))
            return Conflict("Ya existe una asignatura con ese nombre.");
        existente.NomAsignatura = nombre;
        await _db.SaveChangesAsync();
        return Ok(existente);
    }

    /// <summary>BORRAR una asignatura y todas sus notas por id de asignatura.</summary>
    [HttpDelete("borrar/{idAsignatura:int}")]
    public async Task<IActionResult> BorrarAsignatura(int idAsignatura)
    {
        var existente = await _db.Asignaturas.FirstOrDefaultAsync(a => a.IdAsignatura == idAsignatura);
        if (existente == null)
            return NotFound("La asignatura no existe.");

        var notas = await _db.Notas.Where(n => n.IdAsignatura == idAsignatura).ToListAsync();
        if (notas.Count > 0)
        {
            _db.Notas.RemoveRange(notas);
            await _db.SaveChangesAsync();
        }
        _db.Asignaturas.Remove(existente);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Asignatura borrada.", idAsignatura });
    }
}
