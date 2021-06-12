using Mirror;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{

  [SerializeField] private GameObject spawnUnitPrefab = null;

  public override void OnServerAddPlayer(NetworkConnection conn)
  {
    base.OnServerAddPlayer(conn);

    GameObject unitSpawnerInstance = Instantiate(spawnUnitPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

    NetworkServer.Spawn(unitSpawnerInstance, conn);
  }
}