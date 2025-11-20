using System.Net;
using System.Net.Http.Json;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public class AcompanhamentosControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AcompanhamentosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededAcompanhamentos()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/acompanhamentos?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedAcompanhamentoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Descricao == "Exame agendado para 15/01.");
    }

    [Fact]
    public async Task Create_ReturnsCreatedAcompanhamento()
    {
        _factory.ResetDatabase();

        var request = new AcompanhamentoRequestDto
        {
            EncaminhamentoId = 1,
            DataEvento = DateTime.UtcNow,
            TipoEvento = "Observacao",
            Descricao = "Paciente confirmou presença.",
            AnexoUrl = "http://example.com/confirmacao"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/acompanhamentos", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<AcompanhamentoResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Observacao", resource!.Data!.TipoEvento);
        Assert.Equal("Paciente confirmou presença.", resource.Data.Descricao);

        var getResponse = await _client.GetAsync($"/api/v1/acompanhamentos/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedAcompanhamentoResponse
    {
        public List<ResourceResponse<AcompanhamentoResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
