using System.Text.Json.Serialization;

namespace AlumnosApi.Models;

public class Nota
{
    public int IdNota { get; set; }

    // En el JSON se llama "nota" (asi lo envia la app)
    [JsonPropertyName("nota")]
    public decimal? Valor { get; set; }

    public int? NNota { get; set; }
    public int CodAlumno { get; set; }
    public int IdAsignatura { get; set; }
}
