using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;

namespace MindCareAi.Services.Interfaces;

public interface IMindCheckAiService
{
    Task<MindCheckAiAnalysisResponseDto> AnalyzeAsync(MindCheckAiAnalysisRequestDto request, CancellationToken cancellationToken = default);
}
