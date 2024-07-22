namespace Requests
{
    public class CreateGameRequest
    {
        public string PlayerName { get; set; }
        public string PlayerId { get; internal set; }
    }
}