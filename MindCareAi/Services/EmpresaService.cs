using Microsoft.EntityFrameworkCore;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services;

public sealed class EmpresaService(MindCareContext context) : IEmpresaService
{
    private static (int Page, int Size) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<EmpresaResponseDto> CreateAsync(EmpresaRequestDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await context.Empresas.AnyAsync(e => e.Cnpj == dto.Cnpj, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Já existe uma empresa cadastrada com este CNPJ");
        }

        var entity = dto.ToEntity();
        context.Empresas.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<EmpresaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Empresas.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<EmpresaResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Empresas.AsNoTracking();
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(e => e.ToResponseDto()).ToList();
        return new PagedResult<EmpresaResponseDto>(dtos, page, size, total);
    }

    public async Task<EmpresaResponseDto?> UpdateAsync(int id, EmpresaRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Empresas.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (!entity.Cnpj.Equals(dto.Cnpj, StringComparison.Ordinal) && await context.Empresas.AnyAsync(e => e.Cnpj == dto.Cnpj, cancellationToken))
        {
            throw new InvalidOperationException("Já existe outra empresa com este CNPJ");
        }

        entity.Cnpj = dto.Cnpj;
        entity.Nome = dto.Nome;
        entity.PlanoSaude = dto.PlanoSaude;

        await context.SaveChangesAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Empresas.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        context.Empresas.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
