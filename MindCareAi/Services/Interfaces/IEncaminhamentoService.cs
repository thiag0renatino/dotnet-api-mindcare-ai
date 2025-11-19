using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface IEncaminhamentoService
{
    Task<EncaminhamentoResponseDto> CreateAsync(EncaminhamentoRequestDto dto, CancellationToken cancellationToken = default);
    Task<EncaminhamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<EncaminhamentoResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<PagedResult<EncaminhamentoResponseDto>> GetByTriagemAsync(int triagemId, int page, int size, CancellationToken cancellationToken = default);
    Task<EncaminhamentoResponseDto?> UpdateAsync(int id, EncaminhamentoRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<EncaminhamentoRecomendadoDto>> GetRecomendadosAsync(int empresaId, string? especialidade, int page, int size, CancellationToken cancellationToken = default);
}
