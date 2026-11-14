using AlumnosApi.Data;
using AlumnosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsignaturasController : ControllerBase
{
    private readonly AlumnosContext _db;
    public AsignaturasController(AlumnosContext db) => _db = db;

    // GET /api/asignaturas - Lista todas las asignaturas
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _db.Asignaturas.OrderBy(a => a.IdAsignatura).ToListAsync();
        return Ok(lista);
    }

    // POST /api/asignaturas - Crea una asignatura (ID autogenerado)
    [HttpPost]
    public async Task<IActionResult> Crear(Asignatura asignatura)
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
    [HttpPut]
    public async Task<IActionResult> Editar(Asignatura asignatura)
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
}
