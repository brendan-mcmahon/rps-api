using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using rps_api;

namespace EventHandlers
{
    public class DisconnectHandler(GameService gameService, AmazonApiGatewayManagementApiClient webSocketClient, AmazonDynamoDBClient dynamoDbClient)
    : EventHandlerBase(gameService, webSocketClient, dynamoDbClient)
    {
        public async Task<APIGatewayProxyResponse> HandleDisconnect(APIGatewayProxyRequest request)
        {
            var connectionId = request.RequestContext.ConnectionId;

            var deleteItemRequest = new DeleteItemRequest
            {
                TableName = "rps-games",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "PK", new AttributeValue { S = $"CONNECTION#{connectionId}" } }
                }
            };

            try
            {
                await _dynamoDbClient.DeleteItemAsync(deleteItemRequest);
                return new APIGatewayProxyResponse { StatusCode = 200, Body = "Disconnected" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting connection: {ex.Message}");
                return new APIGatewayProxyResponse { StatusCode = 500, Body = "Failed to disconnect" };
            }
        }
    }
}