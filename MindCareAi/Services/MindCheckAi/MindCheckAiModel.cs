using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;
using MindCareAi.Domain.Enums;

namespace MindCareAi.Services.MindCheckAi;

public sealed class MindCheckAiModel
{
    private readonly MLContext _mlContext = new(seed: 556_934);
    private readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> _engine;

    public MindCheckAiModel()
    {
        var trainingView = _mlContext.Data.LoadFromEnumerable(TrainingRows);
        var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(ModelInput.Text))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("Label"))
            .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var model = pipeline.Fit(trainingView);
        _engine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(
            () => _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model));
    }

    internal MindCheckAiModelResult Analyze(MindCheckAiModelContext context)
    {
        var text = BuildInputText(context);
        var prediction = _engine.Value.Predict(new ModelInput { Text = text });
        var risco = ParseRisco(prediction.PredictedLabel);
        var recommendation = MindCheckAiRecommendationCatalog.For(risco, context);
        var confidence = ConfidenceFromScores(prediction.Score);

        return new MindCheckAiModelResult(
            risco,
            recommendation.Sugestoes,
            recommendation.Encaminhamentos,
            recommendation.Justificativa,
            confidence);
    }

    private static TriagemRisco ParseRisco(string label)
    {
        if (Enum.TryParse<TriagemRisco>(label, true, out var risco))
        {
            return risco;
        }

        return TriagemRisco.Baixo;
    }

    private static double ConfidenceFromScores(float[]? scores)
    {
        if (scores is not { Length: > 0 }) return 0.5;
        var exp = scores.Select(value => Math.Exp(value)).ToArray();
        var sum = exp.Sum();
        if (sum <= 0) return 0.5;
        return exp.Max() / sum;
    }

    private static string BuildInputText(MindCheckAiModelContext context)
    {
        var sb = new StringBuilder();
        sb.Append("relato:").Append(context.Relato).Append(' ');
        if (context.Sintomas.Count > 0)
        {
            sb.Append("sintomas:").Append(string.Join(' ', context.Sintomas)).Append(' ');
        }

        if (!string.IsNullOrWhiteSpace(context.Humor))
        {
            sb.Append("humor:").Append(context.Humor).Append(' ');
        }

        if (!string.IsNullOrWhiteSpace(context.Rotina))
        {
            sb.Append("rotina:").Append(context.Rotina);
        }

        return sb.ToString();
    }

    private static IEnumerable<ModelInput> TrainingRows => new[]
    {
        new ModelInput { Text = "ansiedade crise panico isolamento insonia exaustao", Label = "Alto" },
        new ModelInput { Text = "ideacao suicida desesperanca medo intenso falta sono", Label = "Alto" },
        new ModelInput { Text = "crise choro constante irritacao extrema perda apetite", Label = "Alto" },
        new ModelInput { Text = "estresse ocupacional fadiga dificuldade concentrar", Label = "Moderado" },
        new ModelInput { Text = "humor oscilando preocupacao tarefas aperto peito", Label = "Moderado" },
        new ModelInput { Text = "isolamento social queda produtividade preocupacao diaria", Label = "Moderado" },
        new ModelInput { Text = "sono adequado pratica atividade fisica leve", Label = "Baixo" },
        new ModelInput { Text = "relato motivado equilibrio rotina hobbies", Label = "Baixo" },
        new ModelInput { Text = "bom humor suporte familia", Label = "Baixo" }
    };

    private sealed class ModelInput
    {
        public string Text { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    private sealed class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel { get; set; } = string.Empty;

        public float[] Score { get; set; } = Array.Empty<float>();
    }
}

internal sealed class MindCheckAiModelContext
{
    public MindCheckAiModelContext(string relato, IReadOnlyCollection<string>? sintomas, string? humor, string? rotina)
    {
        Relato = string.IsNullOrWhiteSpace(relato) ? string.Empty : relato.Trim();
        Sintomas = (sintomas ?? Array.Empty<string>())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLowerInvariant())
            .ToArray();
        Humor = string.IsNullOrWhiteSpace(humor) ? null : humor.Trim();
        Rotina = string.IsNullOrWhiteSpace(rotina) ? null : rotina.Trim();
    }

    public string Relato { get; }
    public IReadOnlyCollection<string> Sintomas { get; }
    public string? Humor { get; }
    public string? Rotina { get; }
}

internal sealed record MindCheckAiModelResult(
    TriagemRisco Risco,
    IReadOnlyList<string> Sugestoes,
    IReadOnlyList<string> Encaminhamentos,
    string Justificativa,
    double Confianca);

internal sealed record MindCheckAiRecommendation(
    IReadOnlyList<string> Sugestoes,
    IReadOnlyList<string> Encaminhamentos,
    string Justificativa);

internal static class MindCheckAiRecommendationCatalog
{
    public static MindCheckAiRecommendation For(TriagemRisco risco, MindCheckAiModelContext context)
    {
        return risco switch
        {
            TriagemRisco.Alto => BuildHighRisk(context),
            TriagemRisco.Moderado => BuildModerateRisk(context),
            _ => BuildLowRisk(context)
        };
    }

    private static MindCheckAiRecommendation BuildHighRisk(MindCheckAiModelContext context)
    {
        var justificativa = BaseJustification("sintomas intensos", context);
        return new MindCheckAiRecommendation(
            new List<string>
            {
                "Ative rede de apoio imediata e mantenha contato diario.",
                "Agende consulta psiquiatrica para avaliacao medicamentosa.",
                "Sugira afastamento breve do trabalho para foco no tratamento."
            },
            new List<string>
            {
                "Avaliacao psiquiatrica emergencial",
                "Plano de seguranca e plantao psicologico"
            },
            justificativa);
    }

    private static MindCheckAiRecommendation BuildModerateRisk(MindCheckAiModelContext context)
    {
        var justificativa = BaseJustification("oscilacoes emocionais e sobrecarga", context);
        return new MindCheckAiRecommendation(
            new List<string>
            {
                "Reforce higiene do sono e pausas programadas durante a jornada.",
                "Indique psicoterapia breve focada em estresse ocupacional.",
                "Combine check-ins semanais com o RH para monitorar evolucao."
            },
            new List<string>
            {
                "Psicoterapia breve cognitivo-comportamental"
            },
            justificativa);
    }

    private static MindCheckAiRecommendation BuildLowRisk(MindCheckAiModelContext context)
    {
        var justificativa = BaseJustification("relato estavel, sem gatilhos criticos", context);
        return new MindCheckAiRecommendation(
            new List<string>
            {
                "Manter diario de habitos saudaveis e registro de humor.",
                "Sugerir grupos de conversa ou mentorias internas.",
                "Recomendar micro pausas ao longo da semana."
            },
            Array.Empty<string>(),
            justificativa);
    }

    private static string BaseJustification(string foco, MindCheckAiModelContext context)
    {
        var builder = new StringBuilder();
        builder.Append($"Foco em {foco}. Relato cita '{context.Relato[..Math.Min(context.Relato.Length, 140)]}'.");
        if (context.Sintomas.Count > 0)
        {
            builder.Append(" Sintomas destacados: ")
                .Append(string.Join(", ", context.Sintomas.Take(3)))
                .Append('.');
        }

        if (!string.IsNullOrWhiteSpace(context.Humor))
        {
            builder.Append(" Humor reportado: ").Append(context.Humor).Append('.');
        }

        if (!string.IsNullOrWhiteSpace(context.Rotina))
        {
            builder.Append(" Rotina: ").Append(context.Rotina).Append('.');
        }

        return builder.ToString();
    }
}
