using System.Net;
using System.Net.Http.Json;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public class EmpresasControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public EmpresasControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededEmpresas()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/empresas?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedEmpresaResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Nome == "Clinica Bem Viver");
    }

    [Fact]
    public async Task Create_ReturnsCreatedEmpresa()
    {
        _factory.ResetDatabase();

        var request = new EmpresaRequestDto
        {
            Cnpj = "11122233344455",
            Nome = "Nova Vida Ltda",
            PlanoSaude = "Bradesco"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/empresas", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<EmpresaResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Nova Vida Ltda", resource!.Data!.Nome);
        Assert.Equal("11122233344455", resource.Data.Cnpj);

        var getResponse = await _client.GetAsync($"/api/v1/empresas/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedEmpresaResponse
    {
        public List<ResourceResponse<EmpresaResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
