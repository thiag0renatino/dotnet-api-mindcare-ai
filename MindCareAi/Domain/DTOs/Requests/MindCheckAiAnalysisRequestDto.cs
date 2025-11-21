using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MindCareAi.Domain.DTOs.Requests;

public class MindCheckAiAnalysisRequestDto
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    [MinLength(10)]
    public string Relato { get; set; } = string.Empty;

    public List<string>? Sintomas { get; set; }
    public string? Humor { get; set; }
    public string? Rotina { get; set; }
}
