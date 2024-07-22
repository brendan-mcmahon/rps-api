using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Models;

namespace rps_api;

public class GameService(IAmazonDynamoDB dynamoDbClient)
{
    private readonly DynamoDBContext _context = new(dynamoDbClient);

    public async Task<string> CreateGame(string playerId, string playerName)
    {
        var gameId = GenerateGameId();
        var game = new Game
        {
            GameId = gameId,
            GameStatus = "waiting",
            Player1 = new Player { Id = playerId, Name = playerName }
        };

        await SaveGame(game);
        return gameId;
    }

    public async Task JoinGame(string gameId, string playerId, string playerName)
    {
        var game = await GetGame(gameId);
        game.Player2 = new Player { Id = playerId, Name = playerName };
        game.GameStatus = "Setup";
        await SaveGame(game);
    }

    public async Task SubmitItem(string gameId, string playerId, string item)
    {
        var game = await GetGame(gameId);
        if (game.Player1.Id == playerId)
            game.Player1.SecretItem = item;
        else
            game.Player2.SecretItem = item;

        if (!string.IsNullOrEmpty(game.Player1.SecretItem) && !string.IsNullOrEmpty(game.Player2.SecretItem))
            game.GameStatus = "playing";

        await SaveGame(game);
    }

    public async Task SubmitMove(string gameId, string playerId, string move)
    {
        var game = await GetGame(gameId);
        var currentMove = game.Moves.LastOrDefault(m => m.Item2 == null) ?? new Move();

        if (currentMove.Item1 == null)
        {
            currentMove.Item1 = new MoveItem { Player = playerId, ItemName = move };
            game.Moves.Add(currentMove);
        }
        else
        {
            currentMove.Item2 = new MoveItem { Player = playerId, ItemName = move };
            currentMove.Winner = DetermineWinner(currentMove.Item1.ItemName, currentMove.Item2.ItemName);
            UpdateScores(game, currentMove.Winner);

            if (game.Moves.Count == 4)
            {
                game.GameStatus = "finished";
                // game.Winner = DetermineOverallWinner(game);
            }
        }

        await SaveGame(game);
    }

    private async Task SaveGame(Game game) => await _context.SaveAsync(game);

    private async Task<Game> GetGame(string gameId) => await _context.LoadAsync<Game>(gameId);

    private string GenerateGameId()
    {
        return Guid.NewGuid().ToString();
    }
    private string DetermineWinner(string item1, string item2)
    {
        return "Player1";
    }
    private void UpdateScores(Game game, string winner)
    {
        game.Player1.Score = 1;
    }
}