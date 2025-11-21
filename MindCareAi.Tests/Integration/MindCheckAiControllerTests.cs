using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public sealed class MindCheckAiControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MindCheckAiControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Analyze_HighRisk_ReturnsEncaminhamento()
    {
        _factory.ResetDatabase();

        var request = new MindCheckAiAnalysisRequestDto
        {
            UsuarioId = 1,
            Relato = "Crise de ansiedade com insonia e ideacao suicida nas ultimas noites.",
            Sintomas = new() { "insonia", "falta de apetite", "taquicardia" },
            Humor = "desesperanca",
            Rotina = "sem descanso e isolamento"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/mindcheck-ai/analises", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MindCheckAiAnalysisResponseDto>();
        Assert.NotNull(payload);
        Assert.Equal("Alto", payload!.Analise.Risco);
        Assert.Equal("Alto", payload.Triagem.Risco);
        Assert.NotNull(payload.Encaminhamento);
        Assert.Equal("Alta", payload.Encaminhamento!.Prioridade);
        Assert.True(payload.Analise.Sugestoes.Count > 0);
    }

    [Fact]
    public async Task Analyze_LowRisk_DoesNotCreateEncaminhamento()
    {
        _factory.ResetDatabase();

        var request = new MindCheckAiAnalysisRequestDto
        {
            UsuarioId = 1,
            Relato = "Sono adequado, praticando corrida e relatando bom humor.",
            Sintomas = new() { "sono adequado", "habitos saudaveis" },
            Humor = "positivo",
            Rotina = "exercicios regulares"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/mindcheck-ai/analises", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MindCheckAiAnalysisResponseDto>();
        Assert.NotNull(payload);
        Assert.Equal("Baixo", payload!.Analise.Risco);
        Assert.Null(payload.Encaminhamento);
        Assert.Equal("Baixo", payload.Triagem.Risco);
    }

    [Fact]
    public async Task Analyze_UserNotFound_ReturnsNotFound()
    {
        _factory.ResetDatabase();

        var request = new MindCheckAiAnalysisRequestDto
        {
            UsuarioId = 999,
            Relato = "Relato valido para usuario que nao existe no sistema.",
        };

        var response = await _client.PostAsJsonAsync("/api/v1/mindcheck-ai/analises", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
