using UnityEngine;
using TMPro;
using Mirror;

public class GameOverDisplay : MonoBehaviour {

    [SerializeField] private TMP_Text gameOverText = null;
    [SerializeField] private GameObject gameOverParent = null;

    public void LeaveGame() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        } else {
            NetworkManager.singleton.StopClient();
        }
    }

    private void Start() {
        GameOverHandler.ClientHandleGameOver += HandleClientGameOver;
    }

    private void OnDestory() {
        GameOverHandler.ClientHandleGameOver -= HandleClientGameOver;
    }

    private void HandleClientGameOver(string winner) {
        gameOverText.text = $"{winner} Has Won!";
        gameOverParent.SetActive(true);
    }
}
