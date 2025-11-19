using System.ComponentModel.DataAnnotations;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Entities;

public class Acompanhamento
{
    public int Id { get; set; }

    [Required]
    public DateTime DataEvento { get; set; } = DateTime.UtcNow;

    [Required]
    public AcompanhamentoTipoEvento TipoEvento { get; set; }

    public string? Descricao { get; set; }

    [MaxLength(400)]
    public string AnexoUrl { get; set; } = "N/A";

    [Required]
    public int EncaminhamentoId { get; set; }

    public Encaminhamento? Encaminhamento { get; set; }
}
