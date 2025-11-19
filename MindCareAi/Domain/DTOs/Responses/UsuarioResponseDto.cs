using System.Text.Json.Serialization;

namespace MindCareAi.Domain.DTOs.Responses;

public class UsuarioResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EmpresaResponseDto? Empresa { get; set; }
}
