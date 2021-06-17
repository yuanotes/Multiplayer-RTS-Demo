using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
  [SerializeField] private UnityEvent onSelected = null;
  [SerializeField] private UnityEvent onDeSelected = null;
  [SerializeField] private UnitMovement unitMovement = null;
  [SerializeField] private Targeter targeter = null;
  [SerializeField] private Health health = null;

  public static event Action<Unit> ServerUnitSpawnedEvent;
  public static event Action<Unit> ServerUnitDespawnedEvent;

  public static event Action<Unit> AuthorityUnitSpawnedEvent;
  public static event Action<Unit> AuthorityUnitDespawnedEvent;

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
    ServerUnitSpawnedEvent?.Invoke(this);
    health.ServerDieEvent += onUnitDie;
  }

  public override void OnStopServer()
  {
    ServerUnitDespawnedEvent?.Invoke(this);
    health.ServerDieEvent -= onUnitDie;
  }

  [Server]
  private void onUnitDie() {
    NetworkServer.Destroy(gameObject);
  }

  #endregion

  #region  Client

  public override void OnStartClient()
  {
    if (!isClientOnly) { return; }
    if (!hasAuthority) { return; }
    AuthorityUnitSpawnedEvent?.Invoke(this);
  }

  public override void OnStopClient()
  {
    if (isClientOnly) { return; }
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
