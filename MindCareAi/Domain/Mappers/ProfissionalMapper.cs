using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;

namespace MindCareAi.Domain.Mappers;

public static class ProfissionalMapper
{
    public static ProfissionalResponseDto ToResponseDto(this Profissional entity) =>
        new()
        {
            Id = entity.Id,
            Nome = entity.Nome,
            Especialidade = entity.Especialidade,
            Convenio = entity.Convenio,
            Contato = entity.Contato
        };

    public static Profissional ToEntity(this ProfissionalRequestDto dto) =>
        new()
        {
            Nome = dto.Nome,
            Especialidade = dto.Especialidade,
            Convenio = dto.Convenio,
            Contato = dto.Contato
        };

    public static void UpdateFromDto(this Profissional entity, ProfissionalRequestDto dto)
    {
        entity.Nome = dto.Nome;
        entity.Especialidade = dto.Especialidade;
        entity.Convenio = dto.Convenio;
        entity.Contato = dto.Contato;
    }
}
