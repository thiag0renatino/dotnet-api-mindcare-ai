using System.ComponentModel.DataAnnotations;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Entities;

public class Triagem
{
    public int Id { get; set; }

    [Required]
    public DateTime DataHora { get; set; }

    public string? Relato { get; set; }

    [Required]
    public TriagemRisco Risco { get; set; }

    public string? Sugestao { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    public UsuarioSistema? Usuario { get; set; }

    public ICollection<Encaminhamento> Encaminhamentos { get; set; } = new List<Encaminhamento>();
}
