using System.Net;
using System.Net.Http.Json;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public class ProfissionaisControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProfissionaisControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededProfissionais()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/profissionais?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedProfissionalResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Nome == "Alice Souza");
    }

    [Fact]
    public async Task Create_ReturnsCreatedProfissional()
    {
        _factory.ResetDatabase();

        var request = new ProfissionalRequestDto()
        {
            Nome = "Carla Nogueira",
            Especialidade = "Neurologia",
            Convenio = "Bradesco",
            Contato = "carla@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/profissionais", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<ProfissionalResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Carla Nogueira", resource!.Data!.Nome);
        Assert.Equal("Neurologia", resource.Data.Especialidade);

        var getResponse = await _client.GetAsync($"/api/v1/profissionais/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedProfissional()
    {
        _factory.ResetDatabase();

        var request = new ProfissionalRequestDto
        {
            Nome = "Alice Atualizada",
            Especialidade = "Psicanalise",
            Convenio = "Unimed",
            Contato = "alice@contato.com"
        };

        var response = await _client.PutAsJsonAsync("/api/v1/profissionais/1", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<ProfissionalResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Alice Atualizada", resource!.Data!.Nome);
        Assert.Equal("Psicanalise", resource.Data.Especialidade);
    }

    [Fact]
    public async Task Delete_RemovesProfissional()
    {
        _factory.ResetDatabase();

        var response = await _client.DeleteAsync("/api/v1/profissionais/2");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/api/v1/profissionais/2");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedProfissionalResponse
    {
        public List<ResourceResponse<ProfissionalResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
