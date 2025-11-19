using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Mappers;

public static class TriagemMapper
{
    public static TriagemResponseDto ToResponseDto(this Triagem entity) =>
        new()
        {
            Id = entity.Id,
            DataHora = entity.DataHora,
            Relato = entity.Relato ?? string.Empty,
            Risco = entity.Risco.ToString(),
            Sugestao = entity.Sugestao,
            Usuario = entity.Usuario?.ToResponseDto()
        };

    public static Triagem ToEntity(this TriagemRequestDto dto) =>
        new()
        {
            UsuarioId = dto.UsuarioId,
            DataHora = dto.DataHora,
            Relato = dto.Relato,
            Risco = ParseRisco(dto.Risco),
            Sugestao = dto.Sugestao
        };

    public static void UpdateFromDto(this Triagem entity, TriagemRequestDto dto)
    {
        entity.DataHora = dto.DataHora;
        entity.Relato = dto.Relato;
        entity.Risco = ParseRisco(dto.Risco);
        entity.Sugestao = dto.Sugestao;
    }

    private static TriagemRisco ParseRisco(string value) =>
        Enum.Parse<TriagemRisco>(value, true);
}
