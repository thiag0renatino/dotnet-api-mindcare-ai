using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class AcompanhamentoRequestDto
{
    [Required]
    public int EncaminhamentoId { get; set; }

    [Required]
    public DateTime DataEvento { get; set; }

    [Required]
    [MaxLength(20)]
    public string TipoEvento { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [MaxLength(400)]
    public string? AnexoUrl { get; set; }
}
