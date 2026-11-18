using System.Text.Json.Serialization;

namespace AlumnosApi.Models;

public class Nota
{
    public int IdNota { get; set; }

    [JsonPropertyName("nota")]
    public decimal? Valor { get; set; }

    public int? NNota { get; set; }
    public int CodAlumno { get; set; }
    public int IdAsignatura { get; set; }
}
