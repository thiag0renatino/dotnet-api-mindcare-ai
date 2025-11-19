using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class ProfissionalRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Especialidade { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Convenio { get; set; }

    [MaxLength(160)]
    public string? Contato { get; set; }
}
