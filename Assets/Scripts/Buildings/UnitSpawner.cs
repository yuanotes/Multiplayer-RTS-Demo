using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
  [SerializeField] private GameObject unitPrefab = null;
  [SerializeField] private Transform spawnUnitPosition = null;

  [SerializeField] private Health health = null;


  #region Server

  public override void OnStartServer() {
    health.ServerOnDie += HandlServerDie;
  }

  public override void OnStopServer()
  {
    health.ServerOnDie -= HandlServerDie;
  }

  [Server]
  private void HandlServerDie() {
    NetworkServer.Destroy(gameObject);
  }

  [Command]
  private void CmdSpawnUnit()
  {
    GameObject unit = Instantiate(
        unitPrefab,
    spawnUnitPosition.position,
    spawnUnitPosition.transform.rotation);

    NetworkServer.Spawn(unit, connectionToClient);
  }
  #endregion

  #region Client
  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button != PointerEventData.InputButton.Left) { return; }
    if (!hasAuthority) { return; }
    CmdSpawnUnit();
  }
  #endregion
}