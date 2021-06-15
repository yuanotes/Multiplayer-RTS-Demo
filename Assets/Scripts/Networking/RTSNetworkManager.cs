using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{

  [SerializeField] private GameObject spawnUnitPrefab = null;
  [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

  public override void OnServerAddPlayer(NetworkConnection conn)
  {
    base.OnServerAddPlayer(conn);

    GameObject unitSpawnerInstance = Instantiate(spawnUnitPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

    NetworkServer.Spawn(unitSpawnerInstance, conn);
  }

  public override void OnServerChangeScene(string newSceneName)
  {
    if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) {
      GameOverHandler instance = Instantiate(gameOverHandlerPrefab);
      NetworkServer.Spawn(instance.gameObject);
    }
  }
}
