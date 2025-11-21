using System;
using System.Collections.Generic;
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
public class TriagensControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TriagensControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededTriagens()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/triagens?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedTriagemResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Usuario?.Email == "rafael@example.com");
    }

    [Fact]
    public async Task Create_ReturnsCreatedTriagem()
    {
        _factory.ResetDatabase();

        var request = new TriagemRequestDto
        {
            UsuarioId = 1,
            DataHora = DateTime.UtcNow,
            Relato = "Paciente relatou melhora gradual dos sintomas.",
            Risco = "Baixo",
            Sugestao = "Manter acompanhamento quinzenal."
        };

        var response = await _client.PostAsJsonAsync("/api/v1/triagens", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<TriagemResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Paciente relatou melhora gradual dos sintomas.", resource!.Data!.Relato);
        Assert.Equal("Baixo", resource.Data.Risco);
        Assert.Equal(1, resource.Data.Usuario?.Id);

        var getResponse = await _client.GetAsync($"/api/v1/triagens/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedTriagemResponse
    {
        public List<ResourceResponse<TriagemResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
