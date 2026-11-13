using AlumnosApi.Data;
using AlumnosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotasController : ControllerBase
{
    private readonly AlumnosContext _db;
    public NotasController(AlumnosContext db) => _db = db;

    // Validaciones comunes de una nota
    private static string? Validar(Nota nota)
    {
        if (nota.NNota == null || nota.NNota < 1 || nota.NNota > 3)
            return "N_NOTA debe ser 1, 2 o 3.";
        if (nota.Valor == null || nota.Valor < 1.0m || nota.Valor > 7.0m)
            return "La nota debe estar entre 1.0 y 7.0.";
        if (decimal.Round(nota.Valor.Value, 1) != nota.Valor.Value)
            return "La nota admite solo un decimal.";
        return null;
    }

    // POST /api/notas - Ingresar una nota nueva
    [HttpPost]
    public async Task<IActionResult> Ingresar(Nota nota)
    {
        var error = Validar(nota);
        if (error != null) return BadRequest(error);

        if (!await _db.Alumnos.AnyAsync(a => a.CodAlumno == nota.CodAlumno))
            return NotFound("El alumno no existe.");
        if (!await _db.Asignaturas.AnyAsync(a => a.IdAsignatura == nota.IdAsignatura))
            return NotFound("La asignatura no existe.");

        var existe = await _db.Notas.AnyAsync(n =>
            n.CodAlumno == nota.CodAlumno &&
            n.IdAsignatura == nota.IdAsignatura &&
            n.NNota == nota.NNota);
        if (existe)
            return Conflict("Esa nota ya fue ingresada. Use la edición para modificarla.");

        nota.IdNota = 0;
        _db.Notas.Add(nota);
        await _db.SaveChangesAsync();
        return Ok(nota);
    }

    // PUT /api/notas - Editar el valor de una nota existente
    [HttpPut]
    public async Task<IActionResult> Editar(Nota nota)
    {
        var error = Validar(nota);
        if (error != null) return BadRequest(error);

        var existente = await _db.Notas.FirstOrDefaultAsync(n =>
            n.CodAlumno == nota.CodAlumno &&
            n.IdAsignatura == nota.IdAsignatura &&
            n.NNota == nota.NNota);
        if (existente == null)
            return NotFound("No existe esa nota para el alumno y la asignatura indicados.");

        existente.Valor = nota.Valor;
        await _db.SaveChangesAsync();
        return Ok(existente);
    }

    // GET /api/notas/listado - Alumnos con sus notas y promedio
    [HttpGet("listado")]
    public async Task<IActionResult> Listado()
    {
        var filas = await (
            from n in _db.Notas
            join a in _db.Alumnos on n.CodAlumno equals a.CodAlumno
            join s in _db.Asignaturas on n.IdAsignatura equals s.IdAsignatura
            select new
            {
                n.CodAlumno,
                a.Nombres,
                a.Apellidos,
                n.IdAsignatura,
                s.NomAsignatura,
                n.NNota,
                n.Valor
            }).ToListAsync();

        var resultado = new List<Dictionary<string, object?>>();

        var grupos = filas
            .GroupBy(f => new { f.CodAlumno, f.IdAsignatura })
            .OrderBy(g => g.Key.CodAlumno)
            .ThenBy(g => g.First().NomAsignatura);

        foreach (var g in grupos)
        {
            var primera = g.First();
            var n1 = g.FirstOrDefault(x => x.NNota == 1);
            var n2 = g.FirstOrDefault(x => x.NNota == 2);
            var n3 = g.FirstOrDefault(x => x.NNota == 3);

            var fila = new Dictionary<string, object?>();
            fila["COD_ALUMNO"] = primera.CodAlumno;
            fila["NOMBRE_COMPLETO"] = (primera.Nombres + " " + primera.Apellidos).Trim();
            fila["NOM_ASIGNATURA"] = primera.NomAsignatura;

            // Solo se agregan los campos de las notas que existen
            if (n1 != null) { fila["N_NOTA_1"] = n1.NNota; fila["NOTA_1"] = n1.Valor; }
            if (n2 != null) { fila["N_NOTA_2"] = n2.NNota; fila["NOTA_2"] = n2.Valor; }
            if (n3 != null) { fila["N_NOTA_3"] = n3.NNota; fila["NOTA_3"] = n3.Valor; }

            // El promedio solo si están las 3 notas
            if (n1?.Valor != null && n2?.Valor != null && n3?.Valor != null)
                fila["PROMEDIO"] = Math.Round((n1.Valor.Value + n2.Valor.Value + n3.Valor.Value) / 3m, 1);

            resultado.Add(fila);
        }

        return Ok(resultado);
    }
}
