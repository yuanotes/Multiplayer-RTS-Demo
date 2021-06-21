using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System;

public class RTSPlayer : NetworkBehaviour
{
  [SerializeField] private Building[] buildings = new Building[0];
  [SerializeField] private List<Unit> myUnits = new List<Unit>();
  private List<Building> myBuildings = new List<Building>();

  [SyncVar(hook = nameof(onUpdateResources))]
  private int resources = 500;

  public event Action<int> ClientUpdateResourcesEvent;

  public int GetResources() {
    return resources;
  }
  [Server]
  public void SetResources(int newValue) {
    resources = newValue;
  }

  #region Server
  [Command]
  public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
  {
    Building building = null;
    foreach (Building b in buildings)
    {
      if (b.GetId() == buildingId)
      {
        building = b;
        break;
      }
    }
    if (building == null) { return; }
    GameObject instance = Instantiate(building.gameObject, position, building.gameObject.transform.rotation);
    NetworkServer.Spawn(instance, connectionToClient);
  }

  public override void OnStartServer()
  {
    Unit.ServerSpawnedUnitEvent += onServerSpawnedUnit;
    Unit.ServerDespawnedUnitEvent += onServerDespawnedUnit;
    Building.ServerSpawnBuildingEvent += onServerSpawnedBuilding;
    Building.ServerDespawnBuildingEvent += onServerDespawnedBuilding;
  }

  public override void OnStopServer()
  {
    Unit.ServerSpawnedUnitEvent -= onServerSpawnedUnit;
    Unit.ServerDespawnedUnitEvent -= onServerDespawnedUnit;
    Building.ServerSpawnBuildingEvent -= onServerSpawnedBuilding;
    Building.ServerDespawnBuildingEvent -= onServerDespawnedBuilding;
  }
  private void onServerSpawnedUnit(Unit unit)
  {
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
    {
      return;
    }
    myUnits.Add(unit);
  }

  private void onServerDespawnedUnit(Unit unit)
  {
    if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
    {
      return;
    }
    myUnits.Remove(unit);
  }

  private void onServerSpawnedBuilding(Building building)
  {
    if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
    myBuildings.Add(building);
  }
  private void onServerDespawnedBuilding(Building building)
  {
    if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
    myBuildings.Remove(building);
  }
  #endregion

  #region Client
  public List<Unit> GetMyUnits()
  {
    return myUnits;
  }
  public override void OnStartAuthority()
  {

    if (NetworkServer.active) return;

    Unit.AuthorityUnitSpawnedEvent += onAuthoritySpawnedUnit;
    Unit.AuthorityUnitDespawnedEvent += onAuthorityDespawnedUnit;
    Building.AuthoritySpawnBuildingEvent += onAuthoritySpawnedBuilding;
    Building.AuthorityDespawnBuildingEvent += onAuthorityDespawnedBuilding;
  }

  public override void OnStopClient()
  {
    if (!isClientOnly || !hasAuthority) return;
    Unit.AuthorityUnitSpawnedEvent -= onAuthoritySpawnedUnit;
    Unit.AuthorityUnitDespawnedEvent -= onAuthorityDespawnedUnit;
    Building.AuthoritySpawnBuildingEvent -= onAuthoritySpawnedBuilding;
    Building.AuthorityDespawnBuildingEvent -= onAuthorityDespawnedBuilding;
  }

  private void onAuthoritySpawnedUnit(Unit unit)
  {
    myUnits.Add(unit);
  }

  private void onAuthorityDespawnedUnit(Unit unit)
  {
    myUnits.Remove(unit);
  }

  private void onAuthoritySpawnedBuilding(Building building)
  {
    myBuildings.Add(building);
  }
  public void onAuthorityDespawnedBuilding(Building building)
  {
    myBuildings.Remove(building);
  }

  private void onUpdateResources(int oldValue, int newValue)
  {
    ClientUpdateResourcesEvent?.Invoke(newValue);
  }
  #endregion

}
