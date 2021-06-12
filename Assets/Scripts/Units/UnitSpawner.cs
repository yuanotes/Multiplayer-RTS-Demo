using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
  [SerializeField] private GameObject unitPrefab = null;
  [SerializeField] private Transform spawnUnitPosition = null;


  #region Server
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
