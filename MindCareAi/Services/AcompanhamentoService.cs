using Microsoft.EntityFrameworkCore;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services;

public sealed class AcompanhamentoService(MindCareContext context) : IAcompanhamentoService
{
    private static (int Page, int Size) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<AcompanhamentoResponseDto> CreateAsync(AcompanhamentoRequestDto dto, CancellationToken cancellationToken = default)
    {
        var encaminhamento = await context.Encaminhamentos
            .Include(e => e.Triagem)
            .FirstOrDefaultAsync(e => e.Id == dto.EncaminhamentoId, cancellationToken)
            ?? throw new KeyNotFoundException("Encaminhamento não encontrado");

        var entity = dto.ToEntity();
        entity.Encaminhamento = encaminhamento;
        context.Acompanhamentos.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(a => a.Encaminhamento).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<AcompanhamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Acompanhamentos
            .Include(a => a.Encaminhamento)
            .ThenInclude(e => e.Triagem)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<AcompanhamentoResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Acompanhamentos
            .AsNoTracking()
            .Include(a => a.Encaminhamento)
            .ThenInclude(e => e.Triagem);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(a => a.DataEvento)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(a => a.ToResponseDto()).ToList();
        return new PagedResult<AcompanhamentoResponseDto>(dtos, page, size, total);
    }

    public async Task<PagedResult<AcompanhamentoResponseDto>> GetByEncaminhamentoAsync(int encaminhamentoId, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Acompanhamentos
            .AsNoTracking()
            .Include(a => a.Encaminhamento)
            .ThenInclude(e => e.Triagem)
            .Where(a => a.EncaminhamentoId == encaminhamentoId);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(a => a.DataEvento)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(a => a.ToResponseDto()).ToList();
        return new PagedResult<AcompanhamentoResponseDto>(dtos, page, size, total);
    }

    public async Task<AcompanhamentoResponseDto?> UpdateAsync(int id, AcompanhamentoRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Acompanhamentos
            .Include(a => a.Encaminhamento)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null) return null;

        var encaminhamento = await context.Encaminhamentos
            .FirstOrDefaultAsync(e => e.Id == dto.EncaminhamentoId, cancellationToken)
            ?? throw new KeyNotFoundException("Encaminhamento não encontrado");

        entity.Encaminhamento = encaminhamento;
        entity.DataEvento = dto.DataEvento;
        entity.TipoEvento = Enum.Parse<AcompanhamentoTipoEvento>(dto.TipoEvento, true);
        entity.Descricao = dto.Descricao;
        entity.AnexoUrl = string.IsNullOrWhiteSpace(dto.AnexoUrl) ? entity.AnexoUrl : dto.AnexoUrl!;

        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(a => a.Encaminhamento).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Acompanhamentos.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;

        context.Acompanhamentos.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
