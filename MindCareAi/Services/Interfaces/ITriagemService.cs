using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface ITriagemService
{
    Task<TriagemResponseDto> CreateAsync(TriagemRequestDto dto, CancellationToken cancellationToken = default);
    Task<TriagemResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<TriagemResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<PagedResult<TriagemResponseDto>> GetByUsuarioAsync(int usuarioId, int page, int size, CancellationToken cancellationToken = default);
    Task<TriagemResponseDto?> UpdateAsync(int id, TriagemRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
