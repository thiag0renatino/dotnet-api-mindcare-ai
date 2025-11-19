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

public sealed class EncaminhamentoService(MindCareContext context) : IEncaminhamentoService
{
    private static (int Page, int Size) Normalize(int page, int size)
    {
        page = page < 1 ? 1 : page;
        size = size is < 1 or > 100 ? 10 : size;
        return (page, size);
    }

    public async Task<EncaminhamentoResponseDto> CreateAsync(EncaminhamentoRequestDto dto, CancellationToken cancellationToken = default)
    {
        var triagem = await context.Triagens
            .Include(t => t.Usuario)
            .ThenInclude(u => u!.Empresa)
            .FirstOrDefaultAsync(t => t.Id == dto.TriagemId, cancellationToken)
            ?? throw new KeyNotFoundException("Triagem não encontrada");

        Profissional? profissional = null;
        if (dto.ProfissionalId.HasValue)
        {
            profissional = await context.Profissionais
                .FirstOrDefaultAsync(p => p.Id == dto.ProfissionalId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Profissional não encontrado");
        }

        var entity = dto.ToEntity();
        entity.Triagem = triagem;
        entity.Profissional = profissional;
        context.Encaminhamentos.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(e => e.Triagem).LoadAsync(cancellationToken);
        await context.Entry(entity).Reference(e => e.Profissional).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<EncaminhamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Encaminhamentos
            .Include(e => e.Triagem).ThenInclude(t => t.Usuario)
            .Include(e => e.Profissional)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        return entity?.ToResponseDto();
    }

    public async Task<PagedResult<EncaminhamentoResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Encaminhamentos
            .AsNoTracking()
            .Include(e => e.Triagem).ThenInclude(t => t.Usuario)
            .Include(e => e.Profissional);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(e => e.ToResponseDto()).ToList();
        return new PagedResult<EncaminhamentoResponseDto>(dtos, page, size, total);
    }

    public async Task<PagedResult<EncaminhamentoResponseDto>> GetByTriagemAsync(int triagemId, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var query = context.Encaminhamentos
            .AsNoTracking()
            .Include(e => e.Triagem).ThenInclude(t => t.Usuario)
            .Include(e => e.Profissional)
            .Where(e => e.TriagemId == triagemId);
        
        var total = await query.LongCountAsync(cancellationToken);
        
        var list = await query
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        
        var dtos = list.Select(e => e.ToResponseDto()).ToList();
        return new PagedResult<EncaminhamentoResponseDto>(dtos, page, size, total);
    }

    public async Task<EncaminhamentoResponseDto?> UpdateAsync(int id, EncaminhamentoRequestDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Encaminhamentos
            .Include(e => e.Triagem)
            .Include(e => e.Profissional)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null) return null;

        var triagem = await context.Triagens
            .FirstOrDefaultAsync(t => t.Id == dto.TriagemId, cancellationToken)
            ?? throw new KeyNotFoundException("Triagem não encontrada");

        Profissional? profissional = null;
        if (dto.ProfissionalId.HasValue)
        {
            profissional = await context.Profissionais
                .FirstOrDefaultAsync(p => p.Id == dto.ProfissionalId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Profissional não encontrado");
        }

        entity.Triagem = triagem;
        entity.Profissional = profissional;
        entity.Tipo = Enum.Parse<EncaminhamentoTipo>(dto.Tipo, true);
        if (!string.IsNullOrWhiteSpace(dto.Prioridade))
        {
            entity.Prioridade = Enum.Parse<EncaminhamentoPrioridade>(dto.Prioridade, true);
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = Enum.Parse<EncaminhamentoStatus>(dto.Status, true);
        }

        entity.Exame = string.IsNullOrWhiteSpace(dto.Exame) ? entity.Exame : dto.Exame!;
        entity.Especialidade = string.IsNullOrWhiteSpace(dto.Especialidade) ? entity.Especialidade : dto.Especialidade!;
        entity.Observacao = string.IsNullOrWhiteSpace(dto.Observacao) ? entity.Observacao : dto.Observacao!;

        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entity).Reference(e => e.Triagem).LoadAsync(cancellationToken);
        await context.Entry(entity).Reference(e => e.Profissional).LoadAsync(cancellationToken);
        return entity.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Encaminhamentos.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null) return false;

        context.Encaminhamentos.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PagedResult<EncaminhamentoRecomendadoDto>> GetRecomendadosAsync(int empresaId, string? especialidade, int page, int size, CancellationToken cancellationToken = default)
    {
        (page, size) = Normalize(page, size);
        var empresa = await context.Empresas.FirstOrDefaultAsync(e => e.Id == empresaId, cancellationToken)
                       ?? throw new KeyNotFoundException("Empresa não encontrada");

        var especialidadeFiltro = especialidade ?? string.Empty;
        var convenio = empresa.PlanoSaude ?? string.Empty;

        var query = context.Profissionais
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Especialidade, $"%{especialidadeFiltro}%")
                        && EF.Functions.Like(p.Convenio ?? string.Empty, $"%{convenio}%"));

        var total = await query.LongCountAsync(cancellationToken);
        var list = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var dtos = list.Select(p => new EncaminhamentoRecomendadoDto
        {
            ProfissionalId = p.Id,
            Nome = p.Nome,
            Especialidade = p.Especialidade,
            Contato = p.Contato,
            Convenio = p.Convenio
        }).ToList();

        return new PagedResult<EncaminhamentoRecomendadoDto>(dtos, page, size, total);
    }
}
