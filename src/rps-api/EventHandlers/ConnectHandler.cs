using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;

namespace EventHandlers
{
    public class ConnectHandler(GameService gameService, AmazonApiGatewayManagementApiClient webSocketClient, AmazonDynamoDBClient dynamoDbClient)
    : EventHandlerBase(gameService, webSocketClient, dynamoDbClient)
    {
        public async Task<APIGatewayProxyResponse> HandleConnect(APIGatewayProxyRequest request)
        {
            var connectionId = request.RequestContext.ConnectionId;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var ttl = timestamp + 300;

            var item = new Dictionary<string, AttributeValue>
        {
            { "PK", new AttributeValue { S = $"CONNECTION#{connectionId}" } },
            { "ConnectionId", new AttributeValue { S = connectionId } },
            { "Timestamp", new AttributeValue { N = timestamp.ToString() } },
            { "TTL", new AttributeValue { N = ttl.ToString() } }
        };

            var putItemRequest = new PutItemRequest
            {
                TableName = "rps-games",
                Item = item
            };

            try
            {
                await _dynamoDbClient.PutItemAsync(putItemRequest);

                await SendMessageToClient(connectionId, new { Message = "Connected successfully" });

                return new APIGatewayProxyResponse { StatusCode = 200, Body = "Connected" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving connection: {ex.Message}");
                return new APIGatewayProxyResponse { StatusCode = 500, Body = "Failed to connect" };
            }
        }
    }
}