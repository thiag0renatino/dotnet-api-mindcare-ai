using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Mappers;

public static class UsuarioMapper
{
    public static UsuarioResponseDto ToResponseDto(this UsuarioSistema entity) =>
        new()
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Email = entity.Email,
            Tipo = entity.Tipo.ToString(),
            Empresa = entity.Empresa?.ToResponseDto()
        };

    public static UsuarioSistema ToEntity(this UsuarioRequestDto dto) =>
        new()
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
            Tipo = ParseUsuarioTipo(dto.Tipo),
            EmpresaId = dto.EmpresaId
        };

    public static void UpdateFromDto(this UsuarioSistema entity, UsuarioRequestDto dto)
    {
        entity.Nome = dto.Nome;
        entity.Email = dto.Email;
        entity.Tipo = ParseUsuarioTipo(dto.Tipo);
        entity.EmpresaId = dto.EmpresaId;
        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            entity.Senha = dto.Senha;
        }
    }

    private static UsuarioTipo ParseUsuarioTipo(string value) =>
        Enum.Parse<UsuarioTipo>(value, true);
}
