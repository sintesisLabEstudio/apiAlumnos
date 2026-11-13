using AlumnosApi.Data;  // Importa el contexto de BD
using AlumnosApi.Models;  // Importa los modelos (Alumno, etc.)
using Microsoft.AspNetCore.Mvc;  // Importa controlador base y atributos
using Microsoft.EntityFrameworkCore;  // Importa funciones async de EF Core

namespace AlumnosApi.Controllers;

// ===================================================================
// CONTROLADOR: AlumnosController
// ===================================================================
// Este controlador gestiona todas las operaciones relacionadas con Alumnos
// Recibe peticiones HTTP y responde con datos JSON
// Las rutas serán: /api/alumnos (por la ruta abajo)

// 🎖️ [ApiController] - Indica que esta es un controlador de API
// Activa comportamientos automáticos como validación de modelos
[ApiController]

// 🛣️ [Route("api/[controller]")] - Define la ruta base
// [controller] se reemplaza automáticamente por "Alumnos"
// Resultado: las peticiones van a /api/Alumnos
[Route("api/[controller]")]

// Hereda de ControllerBase (no ControllerBase que es para vistas HTML)
public class AlumnosController : ControllerBase
{
    // ===================================================================
    // Inyección de dependencias
    // ===================================================================
    // _db es la conexión con la base de datos
    // "readonly" significa que no puede cambiar después de asignarse
    // (es seguro y buena práctica)
    private readonly AlumnosContext _db;

    // ===================================================================
    // Constructor
    // ===================================================================
    // Se ejecuta cuando se crea una instancia del controlador
    // AlumnosContext se inyecta automáticamente desde Program.cs
    // La flecha "=>" es expresión lambda (forma corta de asignar)
    public AlumnosController(AlumnosContext db) => _db = db;

    // ===================================================================
    // MÉTODO 1: Listar todos los alumnos
    // ===================================================================
    // GET /api/alumnos
    // Obtiene TODOS los alumnos de la base de datos

    // 🔍 [HttpGet] - Ve peticiones HTTP GET
    // GET es para LEER datos (no modifica nada)
    [HttpGet]

    // async Task<IActionResult> - Método asincrónico (no bloquea el servidor)
    // IActionResult - Puede devolver distintos tipos de respuesta (Ok, BadRequest, etc.)
    public async Task<IActionResult> listarAlumnos()
    {
        // 📋 OrderBy: Ordena los alumnos por código (menor a mayor)
        // ToListAsync: Ejecuta la consulta de forma asincrónica (eficiente)
        var lista = await _db.Alumnos.OrderBy(a => a.CodAlumno).ToListAsync();

        // 200 OK - Devuelve la lista en formato JSON
        // Si está vacía, devuelve una lista vacía []
        return Ok(lista);
    }

    // ===================================================================
    // MÉTODO 2: Agregar un nuevo alumno
    // ===================================================================
    // POST /api/alumnos
    // Recibe datos JSON de un alumno nuevo y lo guarda en la BD

    // 📝 [HttpPost] - Recibe peticiones HTTP POST
    // POST es para CREAR nuevos datos
    [HttpPost]

    // El parámetro "alumno" recibe el JSON automáticamente
    // ASP.NET Core deserializa el JSON a un objeto Alumno
    public async Task<IActionResult> agregarAlumno(Alumno alumno)
    {
        // ===================================================================
        // VALIDACIONES - Verificar que los datos sean correctos
        // ===================================================================
        // No queremos guarbar datos "basura" en la BD

        // ❌ Validación 1: El código debe ser mayor a 0
        // Un alumno sin código no tiene sentido
        if (alumno.CodAlumno <= 0)
            // 400 Bad Request - El cliente envió datos inválidos
            return BadRequest("El código de alumno debe ser mayor a 0.");

        // ❌ Validación 2: Nombres y Apellidos son obligatorios
        // string.IsNullOrWhiteSpace: verifica si está vacío o solo tiene espacios
        if (string.IsNullOrWhiteSpace(alumno.Nombres) || string.IsNullOrWhiteSpace(alumno.Apellidos))
            return BadRequest("Nombres y apellidos son obligatorios.");

        // ❌ Validación 3: No puede haber dos alumnos con el mismo código
        // AnyAsync: devuelve true si existe al menos uno que cumple la condición
        if (await _db.Alumnos.AnyAsync(a => a.CodAlumno == alumno.CodAlumno))
            // 409 Conflict - Ya existe un recurso con esa identificación
            return Conflict($"Ya existe un alumno con código {alumno.CodAlumno}.");

        // ===================================================================
        // LIMPIAR DATOS - Preparar antes de guardar
        // ===================================================================

        // Trim(): elimina espacios al inicio y final
        // Ejemplo: "  Juan  " se convierte en "Juan"
        alumno.Nombres = alumno.Nombres.Trim();
        alumno.Apellidos = alumno.Apellidos.Trim();

        // ===================================================================
        // GUARDAR EN LA BASE DE DATOS
        // ===================================================================

        // Add(): añade el alumno a la lista de cambios pendientes
        // (aún NO se guarda en la BD)
        _db.Alumnos.Add(alumno);

        // SaveChangesAsync(): ejecuta todos los cambios en la BD de forma asincrónica
        // Aquí es cuando se ejecuta el INSERT en SQL Server
        await _db.SaveChangesAsync();

        // 200 OK - El alumno se guardó exitosamente
        // Devuelve el objeto alumno que se acaba de crear (con sus datos)
        return Ok(alumno);
    }
}
