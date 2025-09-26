using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Elastic.Clients.Elasticsearch;
using System.Text.Json;
using GameIndexerLambda.Entities;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GameIndexerLambda;

public class Function
{
    private static readonly ElasticsearchClient _esClient;

    static Function()
    {
        var endpoint = Environment.GetEnvironmentVariable("ELASTICSEARCH_ENDPOINT");
        if (string.IsNullOrEmpty(endpoint))
        {
            throw new Exception("Variável de ambiente ELASTICSEARCH_ENDPOINT não configurada.");
        }
        var settings = new ElasticsearchClientSettings(new Uri(endpoint));
        _esClient = new ElasticsearchClient(settings);
    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var message in sqsEvent.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Processando mensagem: {message.Body}");

            // A mensagem do SQS contém um envelope do SNS. Precisamos extrair a mensagem real de dentro.
            var snsEnvelope = JsonSerializer.Deserialize<SnsEnvelope>(message.Body);
            var game = JsonSerializer.Deserialize<Game>(snsEnvelope.Message);

            if (game == null || game.Id == Guid.Empty)
            {
                context.Logger.LogError("Não foi possível deserializar o jogo da mensagem.");
                return;
            }

            var response = await _esClient.IndexAsync(game, "games", game.Id.ToString());

            if (!response.IsSuccess())
            {
                context.Logger.LogError("Falha ao indexar jogo {GameId}: {Reason}", game.Id, response.DebugInformation);
                throw new Exception($"Falha na indexação: {response.DebugInformation}");
            }

            context.Logger.LogInformation("Jogo {GameId} indexado com sucesso.", game.Id);
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "Erro catastrófico ao processar mensagem.");
            throw;
        }
    }
}
public class SnsEnvelope
{
    public string Message { get; set; }
}
