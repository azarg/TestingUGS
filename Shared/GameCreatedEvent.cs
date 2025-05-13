namespace Shared;

public class GameCreatedEvent : GameEvent
{
    public string GameId;

    public GameCreatedEvent(string gameId) {
        GameId = gameId;
    }
}
