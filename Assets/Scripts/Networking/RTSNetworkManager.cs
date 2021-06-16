using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{

  [SerializeField] private GameObject spawnUnitPrefab = null;
  [SerializeField] private GameObject gameOverHandlerPrefab = null;

  public override void OnServerAddPlayer(NetworkConnection conn)
  {
    base.OnServerAddPlayer(conn);
    GameObject unitSpawnerInstance = Instantiate(spawnUnitPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
    NetworkServer.Spawn(unitSpawnerInstance, conn);
  }

  public override void OnServerSceneChanged(string newSceneName)
  {
      if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) {
          GameObject instance = Instantiate(gameOverHandlerPrefab);
          NetworkServer.Spawn(instance);
    }
  }
}
