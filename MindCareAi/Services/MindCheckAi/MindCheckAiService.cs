using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;
using MindCareAi.Domain.Exceptions;
using MindCareAi.Domain.Mappers;
using MindCareAi.Infrastructure.Data;
using MindCareAi.Services.Interfaces;

namespace MindCareAi.Services.MindCheckAi;

public sealed class 
    MindCheckAiService : IMindCheckAiService
{
    private readonly MindCareContext _context;
    private readonly MindCheckAiModel _model;
    private readonly ILogger<MindCheckAiService> _logger;

    public MindCheckAiService(MindCareContext context, ILogger<MindCheckAiService> logger, MindCheckAiModel? model = null)
    {
        _context = context;
        _logger = logger;
        _model = model ?? new MindCheckAiModel();
    }

    public async Task<MindCheckAiAnalysisResponseDto> AnalyzeAsync(
        MindCheckAiAnalysisRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Empresa)
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado");

        var prompt = MindCheckAiPromptBuilder.Build(request, usuario.Nome);
        var context = new MindCheckAiModelContext(request.Relato, request.Sintomas ?? new List<string>(), request.Humor, request.Rotina);
        var modelResult = _model.Analyze(context);
        _logger.LogInformation("MindCheck AI - usuario {UsuarioId} classificado como {Risco} (confianca {Confianca:P0})",
            usuario.Id,
            modelResult.Risco,
            modelResult.Confianca);

        ValidatePayload(modelResult);

        var triagem = new Triagem
        {
            UsuarioId = usuario.Id,
            Usuario = usuario,
            DataHora = DateTime.UtcNow,
            Relato = request.Relato.Trim(),
            Risco = modelResult.Risco,
            Sugestao = string.Join("; ", modelResult.Sugestoes)
        };

        _context.Triagens.Add(triagem);
        await _context.SaveChangesAsync(cancellationToken);

        var triagemDto = triagem.ToResponseDto();
        EncaminhamentoResponseDto? encaminhamentoDto = null;

        if (modelResult.Risco is TriagemRisco.Moderado or TriagemRisco.Alto)
        {
            var encaminhamento = BuildEncaminhamento(modelResult, triagem);
            encaminhamento.Triagem = triagem;
            _context.Encaminhamentos.Add(encaminhamento);
            await _context.SaveChangesAsync(cancellationToken);
            encaminhamentoDto = encaminhamento.ToResponseDto();
        }

        var payload = new MindCheckAiPayloadDto
        {
            Prompt = prompt,
            Risco = modelResult.Risco.ToString(),
            Sugestoes = modelResult.Sugestoes.ToList(),
            Encaminhamentos = modelResult.Encaminhamentos.ToList(),
            Justificativa = modelResult.Justificativa,
            Confianca = Math.Round(modelResult.Confianca, 3)
        };

        return new MindCheckAiAnalysisResponseDto
        {
            Analise = payload,
            Triagem = triagemDto,
            Encaminhamento = encaminhamentoDto
        };
    }

    private static void ValidatePayload(MindCheckAiModelResult result)
    {
        if (string.IsNullOrWhiteSpace(result.Justificativa) || result.Sugestoes.Count == 0)
        {
            throw new MindCheckAiException("MindCheck AI retornou dados invalidos.");
        }
    }

    private static Encaminhamento BuildEncaminhamento(MindCheckAiModelResult result, Triagem triagem)
    {
        var prioridade = result.Risco == TriagemRisco.Alto
            ? EncaminhamentoPrioridade.Alta
            : EncaminhamentoPrioridade.Media;
        var tipo = result.Risco == TriagemRisco.Alto
            ? EncaminhamentoTipo.Especialidade
            : EncaminhamentoTipo.Profissional;

        var especialidade = result.Risco == TriagemRisco.Alto ? "Psiquiatria" : "Psicologia";
        var exame = result.Risco == TriagemRisco.Alto
            ? "Avaliacao psiquiatrica"
            : "Sessao de psicoterapia";

        var observacao = $"Risco {result.Risco} identificado pela MindCheck AI. Confianca: {(result.Confianca * 100):F0}% - {result.Justificativa}";

        return new Encaminhamento
        {
            TriagemId = triagem.Id,
            Tipo = tipo,
            Especialidade = especialidade,
            Exame = exame,
            Prioridade = prioridade,
            Observacao = Truncate(observacao, 380),
            Status = EncaminhamentoStatus.Pendente
        };
    }

    private static string Truncate(string value, int limit)
    {
        if (string.IsNullOrWhiteSpace(value)) return "N/A";
        return value.Length <= limit ? value : value[..limit];
    }
}
