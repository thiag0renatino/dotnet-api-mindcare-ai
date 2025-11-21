using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Exceptions;
using MindCareAi.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MindCareAi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/mindcheck-ai/analises")]
[Produces("application/json")]
[Tags("MindCheck AI")]
public sealed class MindCheckAiController(IMindCheckAiService service) : ControllerBase
{
    private readonly IMindCheckAiService _service = service;

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(MindCheckAiAnalysisResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [SwaggerOperation(Summary = "Analisa relato com MindCheck AI", Description = "Executa triagem inteligente via modelo ML.NET, persiste a triagem e retorna encaminhamento quando necessário." )]
    public async Task<ActionResult<MindCheckAiAnalysisResponseDto>> Analyze(
        [FromBody] MindCheckAiAnalysisRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _service.AnalyzeAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (MindCheckAiException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }
}
