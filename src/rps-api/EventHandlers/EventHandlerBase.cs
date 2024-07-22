using System.Text;
using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using rps_api;

namespace EventHandlers
{
    public abstract class EventHandlerBase(GameService gameService, AmazonApiGatewayManagementApiClient webSocketClient, AmazonDynamoDBClient dynamoDbClient)
    {
        private readonly AmazonApiGatewayManagementApiClient _webSocketClient = webSocketClient;
        protected readonly AmazonDynamoDBClient _dynamoDbClient = dynamoDbClient;
        protected readonly GameService _gameService = gameService;

        protected async Task SendMessageToClient(string connectionId, object message)
        {
            var postConnectionRequest = new PostToConnectionRequest
            {
                ConnectionId = connectionId,
                Data = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)))
            };

            await _webSocketClient.PostToConnectionAsync(postConnectionRequest);
        }

        protected async Task UpdateConnectionWithGame(string connectionId, string gameId, string playerId)
        {
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = "rps-games",
                Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = $"CONN#{connectionId}" } }
            },
                UpdateExpression = "SET GameId = :gameId, PlayerId = :playerId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":gameId", new AttributeValue { S = gameId } },
                { ":playerId", new AttributeValue { S = playerId } }
            }
            };

            await _dynamoDbClient.UpdateItemAsync(updateItemRequest);
        }
    }
}