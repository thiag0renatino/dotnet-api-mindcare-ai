using Asp.Versioning;
using challenge_api_dotnet.Hateoas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MindCareAi.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/acompanhamentos")]
[Produces("application/json")]
[Tags("Acompanhamentos")]
public class AcompanhamentosController(IAcompanhamentoService service) : ControllerBase
{
    private readonly IAcompanhamentoService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Resource<AcompanhamentoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Lista acompanhamentos", Description = "Retorna acompanhamentos paginados com links HATEOAS.")]
    public async Task<ActionResult<PagedResult<Resource<AcompanhamentoResponseDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        if (!HateoasControllerHelper.TryValidatePaging(this, page, size, out var badRequestResult))
            return badRequestResult;

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
    [ProducesResponseType(typeof(Resource<AcompanhamentoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Busca acompanhamento por ID", Description = "Recupera um acompanhamento específico com links HATEOAS.")]
    public async Task<ActionResult<Resource<AcompanhamentoResponseDto>>> GetById(
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

    [HttpGet("encaminhamentos/{encaminhamentoId:int}")]
    [ProducesResponseType(typeof(PagedResult<Resource<AcompanhamentoResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Lista por encaminhamento", Description = "Retorna acompanhamentos de um encaminhamento específico, paginados.")]
    public async Task<ActionResult<PagedResult<Resource<AcompanhamentoResponseDto>>>> GetByEncaminhamento(
        [FromRoute] int encaminhamentoId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        if (!HateoasControllerHelper.TryValidatePaging(this, page, size, out var badRequestResult))
            return badRequestResult;

        var paged = await _service.GetByEncaminhamentoAsync(encaminhamentoId, page, size, cancellationToken);
        var items = paged.Items
            .Select(dto => Url.ToResource(dto, new { id = dto.Id },
                nameof(GetById), nameof(Update), nameof(Delete),
                new[] { Url.CreateLink("list-all", nameof(GetAll), new { page = 1, size = 10 }) }))
            .ToList();

        var links = HateoasControllerHelper.FilteredPagingLinks(
            Url,
            nameof(GetByEncaminhamento),
            new { encaminhamentoId },
            paged.Page,
            paged.PageSize,
            HateoasControllerHelper.TotalPages(paged),
            new[] { Url.CreateLink("list-all", nameof(GetAll), new { page = 1, size = 10 }) });

        var result = HateoasControllerHelper.BuildPagedResult(Url, paged, items, links);

        return Ok(result);
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Resource<AcompanhamentoResponseDto>), StatusCodes.Status201Created)]
    [SwaggerOperation(Summary = "Cria acompanhamento", Description = "Inclui um novo acompanhamento e retorna o recurso com links.")]
    public async Task<ActionResult<Resource<AcompanhamentoResponseDto>>> Create(
        [FromBody] AcompanhamentoRequestDto dto,
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
    [ProducesResponseType(typeof(Resource<AcompanhamentoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Atualiza acompanhamento", Description = "Atualiza um acompanhamento existente pelo ID.")]
    public async Task<ActionResult<Resource<AcompanhamentoResponseDto>>> Update(
        [FromRoute] int id,
        [FromBody] AcompanhamentoRequestDto dto,
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
    [SwaggerOperation(Summary = "Remove acompanhamento", Description = "Exclui um acompanhamento pelo ID.")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        => (await _service.DeleteAsync(id, cancellationToken)) ? NoContent() : NotFound();
}
