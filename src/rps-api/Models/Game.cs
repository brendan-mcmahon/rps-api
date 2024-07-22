using Amazon.DynamoDBv2.DataModel;

namespace Models;

[DynamoDBTable("rps-games")]
public class Game
{
    [DynamoDBHashKey] // Partition key
    public string GameId { get; set; }

    [DynamoDBProperty]
    public string GameStatus { get; set; }

    [DynamoDBProperty]
    public string Winner { get; set; }

    [DynamoDBProperty]
    public Player Player1 { get; set; }

    [DynamoDBProperty]
    public Player Player2 { get; set; }

    [DynamoDBProperty]
    public List<Move> Moves { get; set; } = new List<Move>();
}




