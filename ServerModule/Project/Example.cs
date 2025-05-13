using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudSave.Model;

namespace HelloWorld;

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config) {
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}

public class MyModule
{
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Auto,
    };

    private readonly ILogger<MyModule> _logger;
    public MyModule(ILogger<MyModule> logger) {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new game and stores the game state in the cloud.
    /// </summary>
    /// <returns>Serialized game created event</returns>
    [CloudCodeFunction("CreateGame")]
    public async Task<string> CreateGame(IGameApiClient gameApiClient, IExecutionContext context)
    {

        // 1. -----  New game state-----
        var gameState = new GameState();

        
        // 2. -----  Create and process a new GameCreatedEvent -----
        string gameId = Guid.NewGuid().ToString();
        var gameCreatedEvent = new GameCreatedEvent(gameId);
        gameState.ProcessEvent(gameCreatedEvent);


        // 3. -----  Save game in Cloud Save -----
        await SaveGame(gameApiClient, context, gameState);


        // 4. -----  Return serialized game created event -----
        return JsonConvert.SerializeObject(gameCreatedEvent, JsonSettings);
    }


    /// <summary>
    /// Retrieves the game state from the cloud.
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns>Serialized play turn event</returns>
    [CloudCodeFunction("PlayTurn")]
    public async Task<string> PlayTurn(IGameApiClient gameApiClient, IExecutionContext context, string gameId) {
        
        //_logger.LogInformation("PlayTurn called for {gameId}", gameId);

        // 1. -----  Retrieve the game state from Cloud Save -----
        var keys = new List<string> { "GameState" };
        var response = await gameApiClient.CloudSaveData
    .GetPrivateCustomItemsAsync(context, context.ServiceToken, context.ProjectId, gameId, keys);
        
        var results = response.Data.Results;
        var item = results.First(r => r.Key == "GameState");
        
        _logger.LogInformation("Load data returned {gameStateJson}", item.Value.ToString());

        // 2. -----  Deserialize the game state -----
        /// ERROR HAPPENS HERE
        var gameState = JsonConvert.DeserializeObject<GameState>(item.Value.ToString() ?? "", JsonSettings);


        // 3. ----- Create and process play turn event -----
        var playTurnEvent = new PlayTurnEvent();
        gameState.ProcessEvent(playTurnEvent);

        // 4 -----  Save game in Cloud Save -----
        await SaveGame(gameApiClient, context, gameState);

        // 5. -----  Return serialized play turn event -----
        return JsonConvert.SerializeObject(playTurnEvent, JsonSettings);
    }


    private async Task SaveGame(IGameApiClient gameApiClient, IExecutionContext context, GameState gameState) {

        string gameStateJson = JsonConvert.SerializeObject(gameState, JsonSettings);

        var response = await gameApiClient.CloudSaveData.SetPrivateCustomItemAsync(context, context.ServiceToken, context.ProjectId, gameState.GameId, new SetItemBody("GameState", gameStateJson));
    }
}


