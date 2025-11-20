using System.Net;
using System.Net.Http.Json;
using MindCareAi.Domain.DTOs.Requests;
using MindCareAi.Domain.DTOs.Responses;
using MindCareAi.Tests.Testing;
using Xunit;

namespace MindCareAi.Tests.Integration;

[Collection("IntegrationTests")]
public class UsuariosControllerTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsuariosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededUsuarios()
    {
        _factory.ResetDatabase();

        var response = await _client.GetAsync("/api/v1/usuarios?page=1&size=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedUsuarioResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload!.Total);
        Assert.Contains(payload.Items, r => r.Data?.Email == "rafael@example.com");
    }

    [Fact]
    public async Task Create_ReturnsCreatedUsuario()
    {
        _factory.ResetDatabase();

        var request = new UsuarioRequestDto
        {
            Nome = "Carla Dias",
            Email = "carla.dias@example.com",
            Senha = "Senha@789",
            Tipo = "User",
            EmpresaId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/usuarios", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var resource = await response.Content.ReadFromJsonAsync<ResourceResponse<UsuarioResponseDto>>();
        Assert.NotNull(resource?.Data);
        Assert.Equal("Carla Dias", resource!.Data!.Nome);
        Assert.Equal("carla.dias@example.com", resource.Data.Email);
        Assert.Equal("User", resource.Data.Tipo);
        Assert.Equal(1, resource.Data.Empresa?.Id);

        var getResponse = await _client.GetAsync($"/api/v1/usuarios/{resource.Data.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    private sealed class ResourceResponse<T>
    {
        public T? Data { get; set; }
        public List<object>? Links { get; set; }
    }

    private sealed class PagedUsuarioResponse
    {
        public List<ResourceResponse<UsuarioResponseDto>> Items { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long Total { get; set; }
    }
}
