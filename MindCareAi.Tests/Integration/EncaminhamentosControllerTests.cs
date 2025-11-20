using System.Net;
using System.Net.Http.Json;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public class EncaminhamentosControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EncaminhamentosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededEncaminhamentos()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/encaminhamentos?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedEncaminhamentoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Observacao == "Paciente necessita exame com urgencia.");
    }

    [Fact]
    public async Task Create_ReturnsCreatedEncaminhamento()
    {
        _factory.ResetDatabase();

        var request = new EncaminhamentoRequestDto
        {
            TriagemId = 1,
            ProfissionalId = 1,
            Tipo = "Exame",
            Exame = "Raio X",
            Especialidade = "Radiologia",
            Prioridade = "Baixa",
            Status = "Pendente",
            Observacao = "Solicitar exame complementar."
        };

        var response = await _client.PostAsJsonAsync("/api/v1/encaminhamentos", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<EncaminhamentoResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Raio X", resource!.Data!.Exame);
        Assert.Equal("Radiologia", resource.Data.Especialidade);
        Assert.Equal("Pendente", resource.Data.Status);

        var getResponse = await _client.GetAsync($"/api/v1/encaminhamentos/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetRecomendados_ReturnsMatchingProfissionais()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/encaminhamentos/empresas/1/recomendados?page=1&size=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedRecomendadoResponse>();
        Assert.NotNull(payload);
        Assert.True(payload!.Total >= 1);
        Assert.Contains(payload.Items, r => r.Data?.Convenio == "Unimed");
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedEncaminhamentoResponse
    {
        public List<ResourceResponse<EncaminhamentoResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }

    private sealed class PagedRecomendadoResponse
    {
        public List<ResourceResponse<EncaminhamentoRecomendadoDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
