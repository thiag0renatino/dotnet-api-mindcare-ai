using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class EmpresaRequestDto
{
    [Required]
    [StringLength(14, MinimumLength = 14)]
    [RegularExpression(@"\d{14}", ErrorMessage = "CNPJ deve conter exatamente 14 dígitos numéricos")]
    public string Cnpj { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? PlanoSaude { get; set; }
}
