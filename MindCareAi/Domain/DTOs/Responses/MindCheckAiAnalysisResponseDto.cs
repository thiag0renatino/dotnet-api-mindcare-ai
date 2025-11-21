using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public sealed class MindCheckAiAnalysisResponseDto
{
    public MindCheckAiPayloadDto Analise { get; set; } = new();
    public TriagemResponseDto Triagem { get; set; } = new();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EncaminhamentoResponseDto? Encaminhamento { get; set; }
}
