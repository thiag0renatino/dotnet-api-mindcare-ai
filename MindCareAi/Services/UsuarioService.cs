using Microsoft.EntityFrameworkCore;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Enums;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services;

public sealed class UsuarioService(MindCareContext context) : IUsuarioService
{
    private static (int Page, int PageSize) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<PagedResult<UsuarioResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Usuarios.AsNoTracking().Include(u => u.Empresa);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        var dtos = list.Select(u => u.ToResponseDto()).ToList();
        
        return new PagedResult<UsuarioResponseDto>(dtos, page, size, total);
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return entity?.ToResponseDto();
    }

    public async Task<UsuarioResponseDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var entity = await context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<UsuarioResponseDto>> GetByEmpresaAsync(int empresaId, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Usuarios
            .AsNoTracking()
            .Include(u => u.Empresa)
            .Where(u => u.EmpresaId == empresaId);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(u => u.ToResponseDto()).ToList();
        return new PagedResult<UsuarioResponseDto>(dtos, page, size, total);
    }

    public async Task<PagedResult<UsuarioResponseDto>> GetByTipoAsync(string tipo, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var tipoEnum = Enum.Parse<UsuarioTipo>(tipo, true);
        
        var query = context.Usuarios
            .AsNoTracking()
            .Include(u => u.Empresa)
            .Where(u => u.Tipo == tipoEnum);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(u => u.ToResponseDto()).ToList();
        return new PagedResult<UsuarioResponseDto>(dtos, page, size, total);
    }

    public async Task<UsuarioResponseDto> CreateAsync(UsuarioRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.ToEntity();
        context.Usuarios.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(u => u.Empresa).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<UsuarioResponseDto?> UpdateAsync(int id, UsuarioRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Usuarios.Include(u => u.Empresa).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.UpdateFromDto(dto);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Usuarios.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;

        context.Usuarios.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
