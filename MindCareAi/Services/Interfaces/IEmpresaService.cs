using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface IEmpresaService
{
    Task<EmpresaResponseDto> CreateAsync(EmpresaRequestDto dto, CancellationToken cancellationToken = default);
    Task<EmpresaResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<EmpresaResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<EmpresaResponseDto?> UpdateAsync(int id, EmpresaRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
