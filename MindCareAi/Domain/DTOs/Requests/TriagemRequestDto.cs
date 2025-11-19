using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class TriagemRequestDto
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime DataHora { get; set; }

    [Required]
    [MinLength(5)]
    public string Relato { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Risco { get; set; } = string.Empty;

    public string? Sugestao { get; set; }
}
