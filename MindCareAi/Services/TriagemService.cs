using Microsoft.EntityFrameworkCore;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Enums;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services;

public sealed class TriagemService(MindCareContext context) : ITriagemService
{
    private static (int Page, int PageSize) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<TriagemResponseDto> CreateAsync(TriagemRequestDto dto, CancellationToken cancellationToken = default)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        var entity = dto.ToEntity();
        entity.Usuario = usuario;
        context.Triagens.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(t => t.Usuario).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<TriagemResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Triagens
            .Include(t => t.Usuario)
            .ThenInclude(u => u!.Empresa)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<TriagemResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Triagens
            .AsNoTracking()
            .Include(t => t.Usuario)
            .ThenInclude(u => u!.Empresa);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(t => t.DataHora)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(t => t.ToResponseDto()).ToList();
        return new PagedResult<TriagemResponseDto>(dtos, page, size, total);
    }

    public async Task<PagedResult<TriagemResponseDto>> GetByUsuarioAsync(int usuarioId, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Triagens
            .AsNoTracking()
            .Include(t => t.Usuario)
            .ThenInclude(u => u!.Empresa)
            .Where(t => t.UsuarioId == usuarioId);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(t => t.DataHora)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(t => t.ToResponseDto()).ToList();
        return new PagedResult<TriagemResponseDto>(dtos, page, size, total);
    }

    public async Task<TriagemResponseDto?> UpdateAsync(int id, TriagemRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Triagens
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null) return null;

        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado");

        entity.Usuario = usuario;
        entity.DataHora = dto.DataHora;
        entity.Relato = dto.Relato;
        entity.Risco = Enum.Parse<TriagemRisco>(dto.Risco, true);
        entity.Sugestao = dto.Sugestao;

        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(t => t.Usuario).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Triagens.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;

        context.Triagens.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
