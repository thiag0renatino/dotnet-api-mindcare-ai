using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public sealed class MindCheckAiPayloadDto
{
    public string Prompt { get; set; } = string.Empty;
    public string Risco { get; set; } = string.Empty;
    public List<string> Sugestoes { get; set; } = new();
    public List<string> Encaminhamentos { get; set; } = new();
    public string Justificativa { get; set; } = string.Empty;
    public double Confianca { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool GerouEncaminhamento => Encaminhamentos.Count > 0;
}
