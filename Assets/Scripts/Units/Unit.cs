using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
  [SerializeField] private int unitCost = 100;
  [SerializeField] private UnityEvent onSelected = null;
  [SerializeField] private UnityEvent onDeSelected = null;
  [SerializeField] private UnitMovement unitMovement = null;
  [SerializeField] private Targeter targeter = null;
  [SerializeField] private Health health = null;

  public static event Action<Unit> ServerSpawnedUnitEvent;
  public static event Action<Unit> ServerDespawnedUnitEvent;

  public static event Action<Unit> AuthorityUnitSpawnedEvent;
  public static event Action<Unit> AuthorityUnitDespawnedEvent;

  public int GetUnitCost() { return unitCost; }
  public UnitMovement GetUnitMovement()
  {
    return unitMovement;
  }

  public Targeter GetTargeter()
  {
    return targeter;
  }

  #region Server
  public override void OnStartServer()
  {
    ServerSpawnedUnitEvent?.Invoke(this);
    health.ServerDieEvent += onUnitDie;
  }

  public override void OnStopServer()
  {
    ServerDespawnedUnitEvent?.Invoke(this);
    health.ServerDieEvent -= onUnitDie;
  }

  [Server]
  private void onUnitDie() {
    NetworkServer.Destroy(gameObject);
  }

  #endregion

  #region  Client

  public override void OnStartAuthority()
  {
    AuthorityUnitSpawnedEvent?.Invoke(this);
  }

  public override void OnStopClient()
  {
    if (!hasAuthority) { return; }
    AuthorityUnitDespawnedEvent?.Invoke(this);
  }


  [Client]
  public void Select()
  {
    if (!hasAuthority) { return; };
    onSelected?.Invoke();

  }
  [Client]
  public void DeSelect()
  {
    if (!hasAuthority) { return; };
    onDeSelected?.Invoke();
  }
  #endregion

}
