namespace MindCareAi.Domain.DTOs.Responses;

public class EncaminhamentoRecomendadoDto
{
    public int ProfissionalId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Especialidade { get; init; } = string.Empty;
    public string? Contato { get; init; }
    public string? Convenio { get; init; }
}
