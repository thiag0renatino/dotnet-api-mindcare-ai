using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface IProfissionalService
{
    Task<ProfissionalResponseDto> CreateAsync(ProfissionalRequestDto dto, CancellationToken cancellationToken = default);
    Task<ProfissionalResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProfissionalResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<PagedResult<ProfissionalResponseDto>> GetByEspecialidadeAsync(string especialidade, int page, int size, CancellationToken cancellationToken = default);
    Task<ProfissionalResponseDto?> UpdateAsync(int id, ProfissionalRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
