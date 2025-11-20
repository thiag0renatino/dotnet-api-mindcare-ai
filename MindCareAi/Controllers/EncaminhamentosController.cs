using Asp.Versioning;
using challenge_api_dotnet.Hateoas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Interfaces;

namespace MindCareAi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/encaminhamentos")]
[Produces("application/json")]
[Tags("Encaminhamentos")]
public class EncaminhamentosController(IEncaminhamentoService service) : ControllerBase
{
    private readonly IEncaminhamentoService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Resource<EncaminhamentoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Resource<EncaminhamentoResponseDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        var paged = await _service.GetPagedAsync(page, size, cancellationToken);
        var items = paged.Items
            .Select(dto => Url.ToResource(dto, new { id = dto.Id },
                nameof(GetById), nameof(Update), nameof(Delete),
                new[] { Url.CreateLink("list", nameof(GetAll), new { page = 1, size = 10 }) }))
            .ToList();

        var result = HateoasControllerHelper.BuildPagedResult(
            Url,
            paged,
            items,
            Url.CollectionLinks(nameof(GetAll), paged.Page, paged.PageSize,
                HateoasControllerHelper.TotalPages(paged), nameof(Create)));

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Resource<EncaminhamentoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Resource<EncaminhamentoResponseDto>>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        if (dto is null) return NotFound();

        var resource = Url.ToResource(dto, new { id },
            nameof(GetById), nameof(Update), nameof(Delete),
            new[] { Url.CreateLink("list", nameof(GetAll), new { page = 1, size = 10 }) });

        return Ok(resource);
    }

    [HttpGet("triagens/{triagemId:int}")]
    [ProducesResponseType(typeof(PagedResult<Resource<EncaminhamentoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Resource<EncaminhamentoResponseDto>>>> GetByTriagem(
        [FromRoute] int triagemId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        var paged = await _service.GetByTriagemAsync(triagemId, page, size, cancellationToken);
        var items = paged.Items
            .Select(dto => Url.ToResource(dto, new { id = dto.Id },
                nameof(GetById), nameof(Update), nameof(Delete),
                new[]
                {
                    Url.CreateLink("list-all", nameof(GetAll), new { page = 1, size = 10 })
                }))
            .ToList();

        var links = HateoasControllerHelper.FilteredPagingLinks(
            Url,
            nameof(GetByTriagem),
            new { triagemId },
            paged.Page,
            paged.PageSize,
            HateoasControllerHelper.TotalPages(paged),
            new[]
            {
                Url.CreateLink("create", nameof(Create), method: "POST"),
                Url.CreateLink("list-all", nameof(GetAll), new { page = 1, size = 10 })
            });

        var result = HateoasControllerHelper.BuildPagedResult(Url, paged, items, links);
        return Ok(result);
    }

    [HttpGet("empresas/{empresaId:int}/recomendados")]
    [ProducesResponseType(typeof(PagedResult<Resource<EncaminhamentoRecomendadoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Resource<EncaminhamentoRecomendadoDto>>>> GetRecomendados(
        [FromRoute] int empresaId,
        [FromQuery] string? especialidade = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        var paged = await _service.GetRecomendadosAsync(empresaId, especialidade, page, size, cancellationToken);
        var items = paged.Items.Select(dto =>
        {
            var links = new List<HateoasLink>
            {
                Url.CreateLink("profissional", nameof(ProfissionaisController.GetById),
                    new { controller = "Profissionais", id = dto.ProfissionalId }),
                Url.CreateLink("create-encaminhamento", nameof(Create), method: "POST")
            };
            return new Resource<EncaminhamentoRecomendadoDto>(dto, links);
        }).ToList();

        var links = HateoasControllerHelper.FilteredPagingLinks(
            Url,
            nameof(GetRecomendados),
            new { empresaId, especialidade },
            paged.Page,
            paged.PageSize,
            HateoasControllerHelper.TotalPages(paged));

        var result = HateoasControllerHelper.BuildPagedResult(Url, paged, items, links);
        return Ok(result);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Resource<EncaminhamentoResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<Resource<EncaminhamentoResponseDto>>> Create(
        [FromBody] EncaminhamentoRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var created = await _service.CreateAsync(dto, cancellationToken);
        var resource = Url.ToResource(created, new { id = created.Id },
            nameof(GetById), nameof(Update), nameof(Delete),
            new[] { Url.CreateLink("list", nameof(GetAll), new { page = 1, size = 10 }) });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, resource);
    }

    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Resource<EncaminhamentoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Resource<EncaminhamentoResponseDto>>> Update(
        [FromRoute] int id,
        [FromBody] EncaminhamentoRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _service.UpdateAsync(id, dto, cancellationToken);
        if (updated is null) return NotFound();

        var resource = Url.ToResource(updated, new { id },
            nameof(GetById), nameof(Update), nameof(Delete),
            new[] { Url.CreateLink("list", nameof(GetAll), new { page = 1, size = 10 }) });

        return Ok(resource);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        => (await _service.DeleteAsync(id, cancellationToken)) ? NoContent() : NotFound();
}
