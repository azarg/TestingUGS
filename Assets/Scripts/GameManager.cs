using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class GameManager : MonoBehaviour
{
    public Button GetGameStateButton;
    public Button SaveGameStateButton;

    private void Awake() {
        GetGameStateButton.onClick.AddListener(async () => await GetGameState());
        SaveGameStateButton.onClick.AddListener(async () => await SaveGameState());
    }

    private async void Start() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task GetGameState() {
        var module = new ServerModuleBindings(CloudCodeService.Instance);
        string response = await module.GetGameState();
        Debug.Log(response);
    }

    private async Task SaveGameState() {
        var module = new ServerModuleBindings(CloudCodeService.Instance);
        string response = await module.SaveGameState();
        Debug.Log(response);
    }
}
