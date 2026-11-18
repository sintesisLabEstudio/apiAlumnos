
using AlumnosApi.Data;  // Importa el contexto de la base de datos
using Microsoft.EntityFrameworkCore;  // Importa Entity Framework para trabajar con BD

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AlumnosContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Alumnos")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    var archivoXml = Path.Combine(AppContext.BaseDirectory, "AlumnosApi.xml");
    if (File.Exists(archivoXml))
        opciones.IncludeXmlComments(archivoXml);
});

var app = builder.Build();




app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();
