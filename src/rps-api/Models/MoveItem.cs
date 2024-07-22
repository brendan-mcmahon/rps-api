using Amazon.DynamoDBv2.DataModel;

namespace Models;

public class MoveItem
{
    [DynamoDBProperty]
    public string Player { get; set; }

    [DynamoDBProperty]
    public string ItemName { get; set; }
}