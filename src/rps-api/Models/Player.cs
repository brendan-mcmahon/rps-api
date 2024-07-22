using Amazon.DynamoDBv2.DataModel;

namespace Models;

public class Player
{
    [DynamoDBProperty]
    public string Id { get; set; }

    [DynamoDBProperty]
    public string Name { get; set; }

    [DynamoDBProperty]
    public int Score { get; set; }

    [DynamoDBProperty]
    public string SecretItem { get; set; }
}