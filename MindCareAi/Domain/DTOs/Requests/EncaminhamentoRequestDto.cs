using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class EncaminhamentoRequestDto
{
    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Exame { get; set; }

    [MaxLength(80)]
    public string? Especialidade { get; set; }

    [MaxLength(10)]
    public string? Prioridade { get; set; }

    [MaxLength(12)]
    public string? Status { get; set; }

    [MaxLength(400)]
    public string? Observacao { get; set; }

    [Required]
    public int TriagemId { get; set; }

    public int? ProfissionalId { get; set; }
}
