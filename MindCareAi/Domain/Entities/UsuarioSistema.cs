using System.ComponentModel.DataAnnotations;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Entities;

public class UsuarioSistema
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    public UsuarioTipo Tipo { get; set; } = UsuarioTipo.User;

    public int EmpresaId { get; set; }

    public Empresa? Empresa { get; set; }

    public ICollection<Triagem> Triagens { get; set; } = new List<Triagem>();
}
