using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public class AcompanhamentoResponseDto
{
    public int Id { get; set; }
    public DateTime DataEvento { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? AnexoUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EncaminhamentoResponseDto? Encaminhamento { get; set; }
}
