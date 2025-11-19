using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Domain.Mappers;

public static class EncaminhamentoMapper
{
    public static EncaminhamentoResponseDto ToResponseDto(this Encaminhamento entity) =>
        new()
        {
            Id = entity.Id,
            Tipo = entity.Tipo.ToString(),
            Exame = entity.Exame,
            Especialidade = entity.Especialidade,
            Prioridade = entity.Prioridade.ToString(),
            Status = entity.Status.ToString(),
            Observacao = entity.Observacao,
            Triagem = entity.Triagem?.ToResponseDto(),
            Profissional = entity.Profissional?.ToResponseDto()
        };

    public static Encaminhamento ToEntity(this EncaminhamentoRequestDto dto) =>
        new()
        {
            Tipo = ParseTipo(dto.Tipo),
            Exame = string.IsNullOrWhiteSpace(dto.Exame) ? "N/A" : dto.Exame!,
            Especialidade = string.IsNullOrWhiteSpace(dto.Especialidade) ? "N/A" : dto.Especialidade!,
            Prioridade = ParsePrioridade(dto.Prioridade) ?? EncaminhamentoPrioridade.Media,
            Status = ParseStatus(dto.Status) ?? EncaminhamentoStatus.Pendente,
            Observacao = string.IsNullOrWhiteSpace(dto.Observacao) ? "N/A" : dto.Observacao!,
            TriagemId = dto.TriagemId,
            ProfissionalId = dto.ProfissionalId
        };

    public static void UpdateFromDto(this Encaminhamento entity, EncaminhamentoRequestDto dto)
    {
        entity.Tipo = ParseTipo(dto.Tipo);
        entity.Exame = string.IsNullOrWhiteSpace(dto.Exame) ? entity.Exame : dto.Exame!;
        entity.Especialidade = string.IsNullOrWhiteSpace(dto.Especialidade) ? entity.Especialidade : dto.Especialidade!;
        entity.Prioridade = ParsePrioridade(dto.Prioridade) ?? entity.Prioridade;
        entity.Status = ParseStatus(dto.Status) ?? entity.Status;
        entity.Observacao = string.IsNullOrWhiteSpace(dto.Observacao) ? entity.Observacao : dto.Observacao!;
        entity.ProfissionalId = dto.ProfissionalId;
    }

    private static EncaminhamentoTipo ParseTipo(string value) =>
        Enum.Parse<EncaminhamentoTipo>(value, true);

    private static EncaminhamentoPrioridade? ParsePrioridade(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : Enum.Parse<EncaminhamentoPrioridade>(value, true);

    private static EncaminhamentoStatus? ParseStatus(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : Enum.Parse<EncaminhamentoStatus>(value, true);
}
