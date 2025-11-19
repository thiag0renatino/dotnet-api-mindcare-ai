using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public class TriagemResponseDto
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public string Relato { get; set; } = string.Empty;
    public string Risco { get; set; } = string.Empty;
    public string? Sugestao { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UsuarioResponseDto? Usuario { get; set; }
}
