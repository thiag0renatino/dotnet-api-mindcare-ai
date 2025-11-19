using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.Entities;

public class Profissional
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Especialidade { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? Convenio { get; set; }

    [MaxLength(160)]
    public string? Contato { get; set; }

    public ICollection<Encaminhamento> Encaminhamentos { get; set; } = new List<Encaminhamento>();
}
