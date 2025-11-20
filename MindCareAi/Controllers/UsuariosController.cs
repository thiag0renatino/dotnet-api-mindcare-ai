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
[Route("api/v{version:apiVersion}/usuarios")]
[Produces("application/json")]
[Tags("Usuarios")]
public class UsuariosController(IUsuarioService service) : ControllerBase
{
    private readonly IUsuarioService _service = service;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<Resource<UsuarioResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Lista usuários", Description = "Retorna usuários paginados com links HATEOAS.")]
    public async Task<ActionResult<PagedResult<Resource<UsuarioResponseDto>>>> GetAll(
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
    [ProducesResponseType(typeof(Resource<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Busca usuário por ID", Description = "Recupera um usuário específico.")]
    public async Task<ActionResult<Resource<UsuarioResponseDto>>> GetById(
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

    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(Resource<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Busca usuário por e-mail", Description = "Recupera um usuário pelo endereço de e-mail.")]
    public async Task<ActionResult<Resource<UsuarioResponseDto>>> GetByEmail(
        [FromRoute] string email,
        CancellationToken cancellationToken = default)
    {
        var dto = await _service.GetByEmailAsync(email, cancellationToken);
        if (dto is null) return NotFound();

        var resource = Url.ToResource(dto, new { id = dto.Id },
            nameof(GetById), nameof(Update), nameof(Delete),
            new[] { Url.CreateLink("list", nameof(GetAll), new { page = 1, size = 10 }) });

        return Ok(resource);
    }

    [HttpGet("empresas/{empresaId:int}")]
    [ProducesResponseType(typeof(PagedResult<Resource<UsuarioResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Lista por empresa", Description = "Retorna usuários vinculados a uma empresa, paginados.")]
    public async Task<ActionResult<PagedResult<Resource<UsuarioResponseDto>>>> GetByEmpresa(
        [FromRoute] int empresaId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        if (!HateoasControllerHelper.TryValidatePaging(this, page, size, out var badRequestResult))
            return badRequestResult;

        var paged = await _service.GetByEmpresaAsync(empresaId, page, size, cancellationToken);
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
            nameof(GetByEmpresa),
            new { empresaId },
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

    [HttpGet("tipos/{tipo}")]
    [ProducesResponseType(typeof(PagedResult<Resource<UsuarioResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Lista por tipo", Description = "Retorna usuários filtrados por tipo (perfil), paginados.")]
    public async Task<ActionResult<PagedResult<Resource<UsuarioResponseDto>>>> GetByTipo(
        [FromRoute] string tipo,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        if (!HateoasControllerHelper.TryValidatePaging(this, page, size, out var badRequestResult))
            return badRequestResult;

        var paged = await _service.GetByTipoAsync(tipo, page, size, cancellationToken);
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
            nameof(GetByTipo),
            new { tipo },
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

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Resource<UsuarioResponseDto>), StatusCodes.Status201Created)]
    [SwaggerOperation(Summary = "Cria usuário", Description = "Inclui um novo usuário.")]
    public async Task<ActionResult<Resource<UsuarioResponseDto>>> Create(
        [FromBody] UsuarioRequestDto dto,
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
    [ProducesResponseType(typeof(Resource<UsuarioResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Atualiza usuário", Description = "Atualiza um usuário existente pelo ID.")]
    public async Task<ActionResult<Resource<UsuarioResponseDto>>> Update(
        [FromRoute] int id,
        [FromBody] UsuarioRequestDto dto,
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
    [SwaggerOperation(Summary = "Remove usuário", Description = "Exclui um usuário pelo ID.")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
        => (await _service.DeleteAsync(id, cancellationToken)) ? NoContent() : NotFound();
}
