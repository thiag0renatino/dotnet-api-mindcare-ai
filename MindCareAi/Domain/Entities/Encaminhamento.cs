using System.ComponentModel.DataAnnotations;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Entities;

public class Encaminhamento
{
    public int Id { get; set; }

    [Required]
    public EncaminhamentoTipo Tipo { get; set; }

    [MaxLength(120)]
    public string Exame { get; set; } = "N/A";

    [MaxLength(80)]
    public string Especialidade { get; set; } = "N/A";

    [Required]
    public EncaminhamentoPrioridade Prioridade { get; set; } = EncaminhamentoPrioridade.Media;

    [Required]
    public EncaminhamentoStatus Status { get; set; } = EncaminhamentoStatus.Pendente;

    [Required, MaxLength(400)]
    public string Observacao { get; set; } = "N/A";

    [Required]
    public int TriagemId { get; set; }

    public Triagem? Triagem { get; set; }

    public int? ProfissionalId { get; set; }

    public Profissional? Profissional { get; set; }

    public ICollection<Acompanhamento> Acompanhamentos { get; set; } = new List<Acompanhamento>();
}
