using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Requests;
using rps_api;

namespace EventHandlers
{
    public class CreateGameHandler(GameService gameService, AmazonApiGatewayManagementApiClient webSocketClient, AmazonDynamoDBClient dynamoDbClient) 
    : EventHandlerBase(gameService, webSocketClient, dynamoDbClient)
    {

        public async Task<APIGatewayProxyResponse> HandleCreateGame(APIGatewayProxyRequest request)
        {
            var payload = JsonSerializer.Deserialize<CreateGameRequest>(request.Body)
                ?? throw new ArgumentNullException($"Invalid request body: {request.Body}");

            var gameId = await _gameService.CreateGame(payload.PlayerId, payload.PlayerName);

            await UpdateConnectionWithGame(request.RequestContext.ConnectionId, gameId, payload.PlayerId);

            await SendMessageToClient(request.RequestContext.ConnectionId, new { GameId = gameId });

            return new APIGatewayProxyResponse { StatusCode = 200 };
        }
    }
}