// ===================================================================
// PROGRAMA PRINCIPAL - Configuración de la API
// ===================================================================
// Este archivo configura toda la aplicación ASP.NET Core Web API
// Es como un "director de orquesta" que organiza cómo funciona todo

using AlumnosApi.Data;  // Importa el contexto de la base de datos
using Microsoft.EntityFrameworkCore;  // Importa Entity Framework para trabajar con BD

// ===================================================================
// PASO 1: Crear el "constructor" de la aplicación
// ===================================================================
// CreateBuilder(args) prepara los servicios que necesitará nuestra API
// Como si estuviéramos preparando los instrumentos de la orquesta
var builder = WebApplication.CreateBuilder(args);

// ===================================================================
// PASO 2: Registrar los servicios en el contenedor de inyección 
// ===================================================================
// Aquí le decimos a la aplicación QUÉ debe usar

// 📊 Conectar con la base de datos SQL Server
// - AddDbContext: Registra nuestro "AlumnosContext" (el intermediario con la BD)
// - UseSqlServer: Especifica que usamos SQL Server (no otra BD)
// - GetConnectionString("Alumnos"): Obtiene la conexión del archivo appsettings.json
builder.Services.AddDbContext<AlumnosContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Alumnos")));

// 🎮 Registrar los controladores (las "rutas" del API)
// Los controladores reciben las peticiones HTTP (GET, POST, PUT, DELETE)
builder.Services.AddControllers();

// 📖 Explorer API (Swagger)
// - AddEndpointsApiExplorer: Detecta automáticamente nuestros endpoints
// - AddSwaggerGen: Genera la documentación interactiva
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================================================================
// PASO 3: Crear la aplicación con las configuraciones anteriores
// ===================================================================
// Aquí se construye la aplicación con TODOS los servicios registrados
// Es como ensamblar la orquesta lista para tocar
var app = builder.Build();

// ===================================================================
// PASO 4: Configurar el "middleware" (procesadores de peticiones)
// ===================================================================
// El middleware procesa cada petición HTTP en este orden:

// 🔒 Redirección HTTPS
// Si alguien intenta conectar por HTTP, lo redirige automáticamente a HTTPS
// Esto mejora la seguridad de la comunicación
// app.UseHttpsRedirection(); //y bloquea el celular conectado por usb


// 📖 Activar Swagger
// - UseSwagger: Genera el archivo JSON con la documentación
// - UseSwaggerUI: Muestra la interfaz web en /swagger
app.UseSwagger();
app.UseSwaggerUI();

// ===================================================================
// PASO 5: Configurar las rutas del API
// ===================================================================

// 🏠 Ruta raíz (cuando accedes a https://localhost:50554/)
// Redirige automáticamente a /swagger para ver la documentación
app.MapGet("/", () => Results.Redirect("/swagger"));

// 🎮 Mapear todos los controladores
// Aquí se registran todas las rutas definidas en los Controllers
// (AlumnosController, AsignaturasController, NotasController)
app.MapControllers();

// ===================================================================
// PASO 6: Iniciar la aplicación
// ===================================================================
// Run() inicia el servidor y espera peticiones HTTP
// La aplicación queda corriendo hasta que la cierres
app.Run();
