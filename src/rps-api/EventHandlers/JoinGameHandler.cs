using System.Text.Json;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Requests;
using rps_api;

namespace EventHandlers
{
    public class JoinGameHandler(GameService gameService, AmazonApiGatewayManagementApiClient webSocketClient, AmazonDynamoDBClient dynamoDbClient) 
    : EventHandlerBase(gameService, webSocketClient, dynamoDbClient)
    {

        public async Task<APIGatewayProxyResponse> HandleJoinGame(APIGatewayProxyRequest request)
        {
            var payload = JsonSerializer.Deserialize<JoinGameRequest>(request.Body)
                ?? throw new ArgumentNullException($"Invalid request body: {request.Body}");

            await _gameService.JoinGame(payload.GameId, payload.PlayerId, payload.PlayerName);

            await UpdateConnectionWithGame(request.RequestContext.ConnectionId, payload.GameId, payload.PlayerId);

            await SendMessageToClient(request.RequestContext.ConnectionId, new { payload.GameId });

            return new APIGatewayProxyResponse { StatusCode = 200 };
        }
    }
}