using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.Entities;

public class Empresa
{
    public int Id { get; set; }

    [Required, StringLength(14, MinimumLength = 14)]
    public string Cnpj { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(120)]
    public string? PlanoSaude { get; set; }

    public ICollection<UsuarioSistema> Usuarios { get; set; } = new List<UsuarioSistema>();
}
