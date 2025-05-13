

namespace Shared;

public class GameState
{
    public string GameId;
    public int TurnIndex;
    public List<GameEvent> GameEvents;

    public GameState() {
        TurnIndex = 0;
        GameId = string.Empty;
        GameEvents = new List<GameEvent>();
    }

    public void ProcessEvent(GameEvent gameEvent) {
        if (gameEvent is GameCreatedEvent gameCreatedEvent) {
            GameId = gameCreatedEvent.GameId;
        }
        else if (gameEvent is PlayTurnEvent playTurnEvent) {
            TurnIndex++;
        }
    }
}
