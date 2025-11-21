using System.Text;
using MindCareAi.Domain.DTOs.Requests;

namespace MindCareAi.Services.MindCheckAi;

internal static class MindCheckAiPromptBuilder
{
    public static string Build(MindCheckAiAnalysisRequestDto request, string usuarioNome)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Voce e a MindCheck AI, especialista em saude mental corporativa.");
        sb.AppendLine("Gere um JSON valido com risco (Baixo|Moderado|Alto), sugestoes (array), encaminhamentos (array) e justificativa.");
        sb.AppendLine("Sempre responda em portugues do Brasil.");
        sb.AppendLine("SCHEMA:");
        sb.AppendLine("{\"risco\":\"\",\"sugestoes\":[\"\"],\"encaminhamentos\":[\"\"],\"justificativa\":\"\"}");
        sb.AppendLine("Contexto do usuario:");
        sb.AppendLine($"usuario: {usuarioNome}");
        sb.AppendLine($"relato: {request.Relato}");

        if (request.Sintomas is { Count: > 0 })
        {
            sb.AppendLine($"sintomas: {string.Join(", ", request.Sintomas)}");
        }

        if (!string.IsNullOrWhiteSpace(request.Humor))
        {
            sb.AppendLine($"humor: {request.Humor}");
        }

        if (!string.IsNullOrWhiteSpace(request.Rotina))
        {
            sb.AppendLine($"rotina: {request.Rotina}");
        }

        return sb.ToString();
    }
}
