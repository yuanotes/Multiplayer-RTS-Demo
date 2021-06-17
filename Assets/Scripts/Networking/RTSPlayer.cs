using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
  [SerializeField] private List<Unit> myUnits = new List<Unit>();

  #region Server
  public override void OnStartServer()
  {
    Unit.ServerUnitSpawnedEvent += onServerUnitSpawned;
    Unit.ServerUnitDespawnedEvent += onServerUnitDespawned;
  }

  public override void OnStopServer()
  {
    Unit.ServerUnitSpawnedEvent -= onServerUnitSpawned;
    Unit.ServerUnitDespawnedEvent -= onServerUnitDespawned;
  }
  private void onServerUnitSpawned(Unit unit)
  {
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
    {
      return;
    }
    myUnits.Add(unit);
  }

  private void onServerUnitDespawned(Unit unit)
  {
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
    {
      return;
    }
    myUnits.Remove(unit);
  }
  #endregion

  #region Client
  public List<Unit> GetMyUnits() {
      return myUnits;
  }
  public override void OnStartClient()
  {
    Unit.AuthorityUnitSpawnedEvent += onAuthorityUnitSpawned;
    Unit.AuthorityUnitDespawnedEvent += onAuthorityUnitDespawned;
  }

  public override void OnStopClient()
  {
    Unit.AuthorityUnitSpawnedEvent -= onAuthorityUnitSpawned;
    Unit.AuthorityUnitDespawnedEvent -= onAuthorityUnitDespawned;
  }

  private void onAuthorityUnitSpawned(Unit unit)
  {
    if (!hasAuthority)
    {
      return;
    }
    myUnits.Add(unit);
  }

  private void onAuthorityUnitDespawned(Unit unit)
  {
    if (!hasAuthority)
    {
      return;
    }
    myUnits.Remove(unit);
  }
  #endregion

}
