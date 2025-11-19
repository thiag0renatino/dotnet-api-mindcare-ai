using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public class EmpresaResponseDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PlanoSaude { get; set; }
}
