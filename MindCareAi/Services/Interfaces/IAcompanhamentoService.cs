using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface IAcompanhamentoService
{
    Task<AcompanhamentoResponseDto> CreateAsync(AcompanhamentoRequestDto dto, CancellationToken cancellationToken = default);
    Task<AcompanhamentoResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<AcompanhamentoResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<PagedResult<AcompanhamentoResponseDto>> GetByEncaminhamentoAsync(int encaminhamentoId, int page, int size, CancellationToken cancellationToken = default);
    Task<AcompanhamentoResponseDto?> UpdateAsync(int id, AcompanhamentoRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
