using Unity.Services.CloudCode.Core;

namespace HelloWorld;

public class MyModule
{
    [CloudCodeFunction("SaveGameState")]
    public string SaveGameState()
    {
        return $"Saved";
    }

    [CloudCodeFunction("GetGameState")]
    public string GetGameState() {
        return $"GameState";
    }
}


