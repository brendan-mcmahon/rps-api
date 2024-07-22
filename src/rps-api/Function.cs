using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using EventHandlers;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace rps_api;
public class Function
{
    private readonly ConnectHandler _connectionHandler;
    private readonly DisconnectHandler _disconnectHandler;
    private readonly CreateGameHandler _createGameHandler;
    private readonly JoinGameHandler _joinGameHandler;

    public Function()
    {
        var dynamoDbClient = new AmazonDynamoDBClient();
        var gameService = new GameService(dynamoDbClient);
        var webSocketClient = new AmazonApiGatewayManagementApiClient();
        _connectionHandler = new ConnectHandler(gameService, webSocketClient, dynamoDbClient);
        _disconnectHandler = new DisconnectHandler(gameService, webSocketClient, dynamoDbClient);
        _createGameHandler = new CreateGameHandler(gameService, webSocketClient, dynamoDbClient);
        _joinGameHandler = new JoinGameHandler(gameService, webSocketClient, dynamoDbClient);
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        return request.RequestContext.RouteKey switch
        {
            "$connect" => await _connectionHandler.HandleConnect(request),
            "$disconnect" => await _disconnectHandler.HandleDisconnect(request),
            "create-game" => await _createGameHandler.HandleCreateGame(request),
            "join-game" => await _joinGameHandler.HandleJoinGame(request),
            // "submit-item" => await HandleSubmitItem(request),
            // "submit-move" => await HandleSubmitMove(request),
            _ => new APIGatewayProxyResponse { StatusCode = 400, Body = "Invalid route" },
        };
    }
}