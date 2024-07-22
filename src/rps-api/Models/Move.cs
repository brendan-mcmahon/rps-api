using Amazon.DynamoDBv2.DataModel;

namespace Models;
public class Move
{
    [DynamoDBProperty]
    public MoveItem Item1 { get; set; }

    [DynamoDBProperty]
    public MoveItem Item2 { get; set; }

    [DynamoDBProperty]
    public string Winner { get; set; }
}