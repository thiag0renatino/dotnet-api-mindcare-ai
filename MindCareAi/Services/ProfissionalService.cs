using Microsoft.EntityFrameworkCore;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services;

public sealed class ProfissionalService(MindCareContext context) : IProfissionalService
{
    private static (int Page, int PageSize) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<ProfissionalResponseDto> CreateAsync(ProfissionalRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.ToEntity();
        context.Profissionais.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<ProfissionalResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Profissionais
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<ProfissionalResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Profissionais.AsNoTracking();
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(p => p.ToResponseDto()).ToList();
        return new PagedResult<ProfissionalResponseDto>(dtos, page, size, total);
    }

    public async Task<PagedResult<ProfissionalResponseDto>> GetByEspecialidadeAsync(string especialidade, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Profissionais
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Especialidade, $"%{especialidade}%"));
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(p => p.ToResponseDto()).ToList();
        return new PagedResult<ProfissionalResponseDto>(dtos, page, size, total);
    }

    public async Task<ProfissionalResponseDto?> UpdateAsync(int id, ProfissionalRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Profissionais.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.UpdateFromDto(dto);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Profissionais.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;

        context.Profissionais.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
