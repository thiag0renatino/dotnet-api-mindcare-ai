using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MindCareAi.Domain.Entities;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Infrastructure.Data;

public class MindCareContext(DbContextOptions<MindCareContext> options) : DbContext(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<UsuarioSistema> Usuarios => Set<UsuarioSistema>();
    public DbSet<Triagem> Triagens => Set<Triagem>();
    public DbSet<Profissional> Profissionais => Set<Profissional>();
    public DbSet<Encaminhamento> Encaminhamentos => Set<Encaminhamento>();
    public DbSet<Acompanhamento> Acompanhamentos => Set<Acompanhamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureEmpresa(modelBuilder.Entity<Empresa>());
        ConfigureUsuario(modelBuilder.Entity<UsuarioSistema>());
        ConfigureTriagem(modelBuilder.Entity<Triagem>());
        ConfigureProfissional(modelBuilder.Entity<Profissional>());
        ConfigureEncaminhamento(modelBuilder.Entity<Encaminhamento>());
        ConfigureAcompanhamento(modelBuilder.Entity<Acompanhamento>());
    }

    private static void ConfigureEmpresa(EntityTypeBuilder<Empresa> entity)
    {
        entity.ToTable("EMPRESA");
        entity.HasKey(e => e.Id).HasName("EMPRESA_PK");
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_EMPRESA_GS.NEXTVAL");
        entity.Property(e => e.Cnpj)
            .IsRequired()
            .HasMaxLength(14)
            .HasColumnName("CNPJ");
        entity.HasIndex(e => e.Cnpj)
            .IsUnique()
            .HasDatabaseName("EMPRESA_CNPJ_UN");
        entity.Property(e => e.Nome)
            .IsRequired()
            .HasMaxLength(120)
            .HasColumnName("NOME");
        entity.Property(e => e.PlanoSaude)
            .HasMaxLength(120)
            .HasColumnName("PLANO_SAUDE");
    }

    private static void ConfigureUsuario(EntityTypeBuilder<UsuarioSistema> entity)
    {
        entity.ToTable("USUARIO_SISTEMA");
        entity.HasKey(e => e.Id).HasName("USUARIO_PK");
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_USUARIO_GS.NEXTVAL");
        entity.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnName("NOME");
        entity.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("EMAIL");
        entity.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("USUARIO_EMAIL_UN");
        entity.Property(e => e.Senha)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("SENHA");
        entity.Property(e => e.Tipo)
            .HasColumnName("TIPO")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<UsuarioTipo>(v, true));
        entity.Property(e => e.EmpresaId)
            .HasColumnName("EMPRESA_ID");
        entity.HasOne(e => e.Empresa)
            .WithMany(e => e.Usuarios)
            .HasForeignKey(e => e.EmpresaId)
            .HasConstraintName("USUARIO_EMPRESA_FK");
    }

    private static void ConfigureTriagem(EntityTypeBuilder<Triagem> entity)
    {
        entity.ToTable("TRIAGEM");
        entity.HasKey(t => t.Id).HasName("TRIAGEM_PK");
        entity.Property(t => t.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_TRIAGEM_GS.NEXTVAL");
        entity.Property(t => t.DataHora)
            .HasColumnName("DATA_HORA")
            .IsRequired();
        entity.Property(t => t.Relato)
            .HasColumnName("RELATO");
        entity.Property(t => t.Risco)
            .HasColumnName("RISCO")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<TriagemRisco>(v, true));
        entity.Property(t => t.Sugestao)
            .HasColumnName("SUGESTAO");
        entity.Property(t => t.UsuarioId)
            .HasColumnName("USUARIO_ID");
        entity.HasOne(t => t.Usuario)
            .WithMany(u => u.Triagens)
            .HasForeignKey(t => t.UsuarioId)
            .HasConstraintName("TRIAGEM_USUARIO_FK");
    }

    private static void ConfigureProfissional(EntityTypeBuilder<Profissional> entity)
    {
        entity.ToTable("PROFISSIONAL");
        entity.HasKey(p => p.Id).HasName("PROFISSIONAL_PK");
        entity.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_PROFISSIONAL_GS.NEXTVAL");
        entity.Property(p => p.Nome)
            .HasColumnName("NOME")
            .HasMaxLength(100)
            .IsRequired();
        entity.Property(p => p.Especialidade)
            .HasColumnName("ESPECIALIDADE")
            .HasMaxLength(80)
            .IsRequired();
        entity.Property(p => p.Convenio)
            .HasColumnName("CONVENIO")
            .HasMaxLength(120);
        entity.Property(p => p.Contato)
            .HasColumnName("CONTATO")
            .HasMaxLength(160);
    }

    private static void ConfigureEncaminhamento(EntityTypeBuilder<Encaminhamento> entity)
    {
        entity.ToTable("ENCAMINHAMENTO");
        entity.HasKey(e => e.Id).HasName("ENCAMINHAMENTO_PK");
        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_ENCAMINHAMENTO_GS.NEXTVAL");
        entity.Property(e => e.Tipo)
            .HasColumnName("TIPO")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EncaminhamentoTipo>(v, true));
        entity.Property(e => e.Exame)
            .HasColumnName("EXAME")
            .HasMaxLength(120)
            .HasDefaultValue("N/A");
        entity.Property(e => e.Especialidade)
            .HasColumnName("ESPECIALIDADE")
            .HasMaxLength(80)
            .HasDefaultValue("N/A");
        entity.Property(e => e.Prioridade)
            .HasColumnName("PRIORIDADE")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EncaminhamentoPrioridade>(v, true))
            .HasDefaultValue(EncaminhamentoPrioridade.Media);
        entity.Property(e => e.Status)
            .HasColumnName("STATUS")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<EncaminhamentoStatus>(v, true))
            .HasDefaultValue(EncaminhamentoStatus.Pendente);
        entity.Property(e => e.Observacao)
            .HasColumnName("OBSERVACAO")
            .HasMaxLength(400)
            .HasDefaultValue("N/A");
        entity.Property(e => e.TriagemId)
            .HasColumnName("TRIAGEM_ID");
        entity.Property(e => e.ProfissionalId)
            .HasColumnName("PROFISSIONAL_ID");
        entity.HasOne(e => e.Triagem)
            .WithMany(t => t.Encaminhamentos)
            .HasForeignKey(e => e.TriagemId)
            .HasConstraintName("ENCAMINHAMENTO_TRIAGEM_FK");
        entity.HasOne(e => e.Profissional)
            .WithMany(p => p.Encaminhamentos)
            .HasForeignKey(e => e.ProfissionalId)
            .HasConstraintName("ENCAMINHAMENTO_PROFISSIONAL_FK");
    }

    private static void ConfigureAcompanhamento(EntityTypeBuilder<Acompanhamento> entity)
    {
        entity.ToTable("ACOMPANHAMENTO");
        entity.HasKey(a => a.Id).HasName("ACOMPANHAMENTO_PK");
        entity.Property(a => a.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("SEQ_ACOMPANHAMENTO_GS.NEXTVAL");
        entity.Property(a => a.DataEvento)
            .HasColumnName("DATA_EVENTO")
            .IsRequired();
        entity.Property(a => a.TipoEvento)
            .HasColumnName("TIPO_EVENTO")
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<AcompanhamentoTipoEvento>(v, true));
        entity.Property(a => a.Descricao)
            .HasColumnName("DESCRICAO");
        entity.Property(a => a.AnexoUrl)
            .HasColumnName("ANEXO_URL")
            .HasMaxLength(400)
            .HasDefaultValue("N/A");
        entity.Property(a => a.EncaminhamentoId)
            .HasColumnName("ENCAMINHAMENTO_ID");
        entity.HasOne(a => a.Encaminhamento)
            .WithMany(e => e.Acompanhamentos)
            .HasForeignKey(a => a.EncaminhamentoId)
            .HasConstraintName("ACOMPANHAMENTO_ENCAMINHAMENTO_FK");
    }
}
