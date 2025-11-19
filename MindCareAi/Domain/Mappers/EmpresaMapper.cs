using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;

namespace MindCareAi.Domain.Mappers;

public static class EmpresaMapper
{
    public static EmpresaResponseDto ToResponseDto(this Empresa entity) =>
        new()
        {
            Id = entity.Id,
            Cnpj = entity.Cnpj,
            Nome = entity.Nome,
            PlanoSaude = entity.PlanoSaude
        };

    public static Empresa ToEntity(this EmpresaRequestDto dto) =>
        new()
        {
            Cnpj = dto.Cnpj,
            Nome = dto.Nome,
            PlanoSaude = dto.PlanoSaude
        };

    public static void UpdateFromDto(this Empresa entity, EmpresaRequestDto dto)
    {
        entity.Nome = dto.Nome;
        entity.PlanoSaude = dto.PlanoSaude;
    }
}
