using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public class EncaminhamentoResponseDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Exame { get; set; }
    public string? Especialidade { get; set; }
    public string? Prioridade { get; set; }
    public string? Status { get; set; }
    public string? Observacao { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TriagemResponseDto? Triagem { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ProfissionalResponseDto? Profissional { get; set; }
}
