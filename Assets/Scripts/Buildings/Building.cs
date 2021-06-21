using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Building : NetworkBehaviour
{
  [SerializeField] private GameObject buildingPreview = null;
  [SerializeField] private Sprite icon = null;
  [SerializeField] private int id = -1;
  [SerializeField] private int price = 0;

  public GameObject GetBuildingPreview() { return buildingPreview; }
  public int GetId() { return id; }
  public Sprite GetIcon() { return icon; }
  public int GetPrice() { return price; }

  public static event Action<Building> ServerSpawnBuildingEvent;
  public static event Action<Building> ServerDespawnBuildingEvent;

  public static event Action<Building> AuthoritySpawnBuildingEvent;
  public static event Action<Building> AuthorityDespawnBuildingEvent;

  #region Server
  public override void OnStartServer()
  {
    ServerSpawnBuildingEvent?.Invoke(this);
  }
  public override void OnStopServer()
  {
    ServerDespawnBuildingEvent?.Invoke(this);
  }
  #endregion

  #region Client
  public override void OnStartAuthority()
  {
    AuthoritySpawnBuildingEvent?.Invoke(this);
  }

  public override void OnStopClient()
  {
    if (!hasAuthority) return;
    AuthorityDespawnBuildingEvent?.Invoke(this);
  }
  #endregion
}
