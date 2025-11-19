using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Services.Pagination;

namespace MindCareAi.Services.Interfaces;

public interface IUsuarioService
{
    Task<PagedResult<UsuarioResponseDto>> GetPagedAsync(int page, int size, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PagedResult<UsuarioResponseDto>> GetByEmpresaAsync(int empresaId, int page, int size, CancellationToken cancellationToken = default);
    Task<PagedResult<UsuarioResponseDto>> GetByTipoAsync(string tipo, int page, int size, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto> CreateAsync(UsuarioRequestDto dto, CancellationToken cancellationToken = default);
    Task<UsuarioResponseDto?> UpdateAsync(int id, UsuarioRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
