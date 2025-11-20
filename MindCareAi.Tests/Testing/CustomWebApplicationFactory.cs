using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;
using MindCareAi.Infrastructure.Data;

namespace MindCareAi.Tests.Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<MindCareContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<MindCareContext>(options =>
            {
                options.UseInMemoryDatabase("MindCareAiTests");
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MindCareContext>();
            context.Database.EnsureCreated();
            SeedDatabase(context);
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MindCareContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        SeedDatabase(context);
    }

    private static void SeedDatabase(MindCareContext context)
    {
        context.Empresas.AddRange(
            new Empresa
            {
                Id = 1,
                Cnpj = "12345678901234",
                Nome = "Clinica Bem Viver",
                PlanoSaude = "Unimed"
            },
            new Empresa
            {
                Id = 2,
                Cnpj = "98765432109876",
                Nome = "Saude Integral Ltda",
                PlanoSaude = "SulAmerica"
            });

        context.Usuarios.AddRange(
            new UsuarioSistema
            {
                Id = 1,
                Nome = "Rafael Almeida",
                Email = "rafael@example.com",
                Senha = "Senha@123",
                Tipo = UsuarioTipo.Admin,
                EmpresaId = 1
            },
            new UsuarioSistema
            {
                Id = 2,
                Nome = "Bianca Ramos",
                Email = "bianca@example.com",
                Senha = "Senha@456",
                Tipo = UsuarioTipo.User,
                EmpresaId = 2
            });

        context.Triagens.AddRange(
            new Triagem
            {
                Id = 1,
                UsuarioId = 1,
                DataHora = new DateTime(2024, 1, 10, 14, 0, 0, DateTimeKind.Utc),
                Relato = "Paciente relatou ansiedade elevada nas últimas semanas.",
                Risco = TriagemRisco.Alto,
                Sugestao = "Agendar consulta urgente."
            },
            new Triagem
            {
                Id = 2,
                UsuarioId = 2,
                DataHora = new DateTime(2024, 1, 9, 9, 30, 0, DateTimeKind.Utc),
                Relato = "Sintomas moderados de estresse ocupacional.",
                Risco = TriagemRisco.Moderado,
                Sugestao = "Encaminhar para acompanhamento semanal."
            });

        context.Encaminhamentos.AddRange(
            new Encaminhamento
            {
                Id = 1,
                TriagemId = 1,
                ProfissionalId = 1,
                Tipo = EncaminhamentoTipo.Exame,
                Exame = "Ressonancia",
                Especialidade = "Neurologia",
                Prioridade = EncaminhamentoPrioridade.Alta,
                Status = EncaminhamentoStatus.Agendado,
                Observacao = "Paciente necessita exame com urgencia."
            },
            new Encaminhamento
            {
                Id = 2,
                TriagemId = 2,
                ProfissionalId = 2,
                Tipo = EncaminhamentoTipo.Especialidade,
                Exame = "Avaliação psiquiátrica",
                Especialidade = "Psiquiatria",
                Prioridade = EncaminhamentoPrioridade.Media,
                Status = EncaminhamentoStatus.Pendente,
                Observacao = "Avaliar ajuste medicamentoso."
            });

        context.Acompanhamentos.AddRange(
            new Acompanhamento
            {
                Id = 1,
                EncaminhamentoId = 1,
                DataEvento = new DateTime(2024, 1, 11, 10, 0, 0, DateTimeKind.Utc),
                TipoEvento = AcompanhamentoTipoEvento.Agendamento,
                Descricao = "Exame agendado para 15/01.",
                AnexoUrl = "http://example.com/agendamento"
            },
            new Acompanhamento
            {
                Id = 2,
                EncaminhamentoId = 1,
                DataEvento = new DateTime(2024, 1, 16, 16, 30, 0, DateTimeKind.Utc),
                TipoEvento = AcompanhamentoTipoEvento.Resultado,
                Descricao = "Resultado disponível no sistema.",
                AnexoUrl = "http://example.com/resultado"
            });

        context.Profissionais.AddRange(
            new Profissional
            {
                Id = 1,
                Nome = "Alice Souza",
                Especialidade = "Psicologia",
                Convenio = "Unimed",
                Contato = "alice@example.com"
            },
            new Profissional
            {
                Id = 2,
                Nome = "Bruno Lima",
                Especialidade = "Psiquiatria",
                Convenio = "SulAmerica",
                Contato = "bruno@example.com"
            });

        context.SaveChanges();
    }
}
