using AlumnosApi.Data;
using AlumnosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlumnosApi.Controllers;

[ApiController]
[Route("api/notas")]
public class NotasController : ControllerBase
{
    private readonly AlumnosContext _db;
    public NotasController(AlumnosContext db) => _db = db;

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

    /// <summary>LISTAR todas las notas registradas.</summary>
    [HttpGet("listar")]
    public async Task<IActionResult> ListarNotas()
    {
        var lista = await _db.Notas.OrderBy(n => n.IdNota).ToListAsync();
        return Ok(lista);
    }

    /// <summary>LISTAR alumnos con sus notas y PROMEDIO (solo con las 3 notas).</summary>
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

            if (n1 != null) { fila["N_NOTA_1"] = n1.NNota; fila["NOTA_1"] = n1.Valor; }
            if (n2 != null) { fila["N_NOTA2"] = n2.NNota; fila["NOTA_2"] = n2.Valor; }
            if (n3 != null) { fila["N_NOTA_3"] = n3.NNota; fila["NOTA_3"] = n3.Valor; }

            if (n1?.Valor != null && n2?.Valor != null && n3?.Valor != null)
                fila["PROMEDIO"] = Math.Round((n1.Valor.Value + n2.Valor.Value + n3.Valor.Value) / 3m, 1);

            resultado.Add(fila);
        }

        return Ok(resultado);
    }

    /// <summary>GRABAR una nota nueva (N_NOTA 1 a 3, nota 1.0 a 7.0).</summary>
    [HttpPost("grabar")]
    public async Task<IActionResult> GrabarNota(Nota nota)
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

    /// <summary>EDITAR el valor de una nota existente.</summary>
    [HttpPut("editar")]
    public async Task<IActionResult> EditarNota(Nota nota)
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

    /// <summary>BORRAR una nota por id de nota.</summary>
    [HttpDelete("borrar/{idNota:int}")]
    public async Task<IActionResult> BorrarNota(int idNota)
    {
        var existente = await _db.Notas.FirstOrDefaultAsync(n => n.IdNota == idNota);
        if (existente == null)
            return NotFound("No existe esa nota.");
        _db.Notas.Remove(existente);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Nota borrada.", idNota });
    }

    /// <summary>BORRAR todas las notas de un alumno en una asignatura.</summary>
    [HttpDelete("borrarPorAlumnoAsignatura/{codAlumno:int}/{idAsignatura:int}")]
    public async Task<IActionResult> BorrarNotasAlumnoAsignatura(int codAlumno, int idAsignatura)
    {
        var notas = await _db.Notas.Where(n =>
            n.CodAlumno == codAlumno && n.IdAsignatura == idAsignatura).ToListAsync();
        if (notas.Count == 0)
            return NotFound("No hay notas para ese alumno y asignatura.");
        _db.Notas.RemoveRange(notas);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Notas borradas.", codAlumno, idAsignatura, cantidad = notas.Count });
    }
}
