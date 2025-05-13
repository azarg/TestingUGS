using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Newtonsoft.Json;
using Shared;

public class GameManager : MonoBehaviour
{
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Auto,
    };

    public Button CreateGameStateButton;
    public Button PlayTurnButton;

    private GameState gameState;

    private void Awake() {
        CreateGameStateButton.onClick.AddListener(async () => await CreateGame());
        PlayTurnButton.onClick.AddListener(async () => await PlayTurn());
    }

    private async void Start() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        gameState = new GameState();
    }

    private async Task CreateGame() {
        // 1. ---- Make server call to create a game ----
        var module = new ServerModuleBindings(CloudCodeService.Instance);
        string response = await module.CreateGame();        
        
        Debug.Log(response);

        // 2. ---- Deserialize the response ----
        var gameCreatedEvent = JsonConvert.DeserializeObject<GameCreatedEvent>(response, JsonSettings);

        // 3. ---- Process the event ----
        gameState.ProcessEvent(gameCreatedEvent);

    }


    private async Task PlayTurn() {
        if (string.IsNullOrEmpty(gameState.GameId)) {
            Debug.Log("Game ID is not set. Please create a game first.");
            return;
        }

        // 1. ---- Make server call to play a turn ----
        var module = new ServerModuleBindings(CloudCodeService.Instance);
        string response = await module.PlayTurn(gameState.GameId);

        Debug.Log(response);

        // 2. ---- Deserialize the response ----
        var playTurnEvent = JsonConvert.DeserializeObject<PlayTurnEvent>(response, JsonSettings);

        // 3. ---- Process the event ----
        gameState.ProcessEvent(playTurnEvent);
    }
}
