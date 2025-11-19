using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class UsuarioRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public int EmpresaId { get; set; }
}
