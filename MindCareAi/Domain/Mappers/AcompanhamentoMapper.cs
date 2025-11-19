using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Mappers;

public static class AcompanhamentoMapper
{
    public static AcompanhamentoResponseDto ToResponseDto(this Acompanhamento entity) =>
        new()
        {
            Id = entity.Id,
            DataEvento = entity.DataEvento,
            TipoEvento = entity.TipoEvento.ToString(),
            Descricao = entity.Descricao,
            AnexoUrl = entity.AnexoUrl,
            Encaminhamento = entity.Encaminhamento?.ToResponseDto()
        };

    public static Acompanhamento ToEntity(this AcompanhamentoRequestDto dto) =>
        new()
        {
            EncaminhamentoId = dto.EncaminhamentoId,
            DataEvento = dto.DataEvento,
            TipoEvento = ParseTipoEvento(dto.TipoEvento),
            Descricao = dto.Descricao,
            AnexoUrl = string.IsNullOrWhiteSpace(dto.AnexoUrl) ? "N/A" : dto.AnexoUrl!
        };

    public static void UpdateFromDto(this Acompanhamento entity, AcompanhamentoRequestDto dto)
    {
        entity.DataEvento = dto.DataEvento;
        entity.TipoEvento = ParseTipoEvento(dto.TipoEvento);
        entity.Descricao = dto.Descricao;
        entity.AnexoUrl = string.IsNullOrWhiteSpace(dto.AnexoUrl) ? entity.AnexoUrl : dto.AnexoUrl!;
    }

    private static AcompanhamentoTipoEvento ParseTipoEvento(string value) =>
        Enum.Parse<AcompanhamentoTipoEvento>(value, true);
}
