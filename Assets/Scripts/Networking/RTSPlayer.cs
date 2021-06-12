using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
  [SerializeField] private List<Unit> myUnits = new List<Unit>();

  #region Server
  public override void OnStartServer()
  {
    Unit.ServerOnUnitSpawned += ServerOnUnitSpawned;
    Unit.ServerOnUnitDespawned += ServerOnUnitDespawned;
  }

  public override void OnStopServer()
  {
    Unit.ServerOnUnitSpawned -= ServerOnUnitSpawned;
    Unit.ServerOnUnitDespawned -= ServerOnUnitDespawned;
  }
  private void ServerOnUnitSpawned(Unit unit)
  {
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
    {
      return;
    }
    myUnits.Add(unit);
  }

  private void ServerOnUnitDespawned(Unit unit)
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
    Unit.AuthorityOnUnitSpawned += AuthorityOnUnitSpawned;
    Unit.AuthorityOnUnitDespawned += AuthorityOnUnitDespawned;
  }

  public override void OnStopClient()
  {
    Unit.AuthorityOnUnitSpawned -= AuthorityOnUnitSpawned;
    Unit.AuthorityOnUnitDespawned -= AuthorityOnUnitDespawned;
  }

  private void AuthorityOnUnitSpawned(Unit unit)
  {
    if (!hasAuthority)
    {
      return;
    }
    myUnits.Add(unit);
  }

  private void AuthorityOnUnitDespawned(Unit unit)
  {
    if (!hasAuthority)
    {
      return;
    }
    myUnits.Remove(unit);
  }
  #endregion

}
