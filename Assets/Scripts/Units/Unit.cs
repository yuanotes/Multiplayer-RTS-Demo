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

  public static event Action<Unit> ServerOnUnitSpawned;
  public static event Action<Unit> ServerOnUnitDespawned;

  public static event Action<Unit> AuthorityOnUnitSpawned;
  public static event Action<Unit> AuthorityOnUnitDespawned;

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
    ServerOnUnitSpawned?.Invoke(this);
    health.ServerOnDie += OnUnitHandleDie;
  }

  public override void OnStopServer()
  {
    ServerOnUnitDespawned?.Invoke(this);
    health.ServerOnDie -= OnUnitHandleDie;
  }

  [Server]
  private void OnUnitHandleDie() {
    NetworkServer.Destroy(gameObject);
  }

  #endregion

  #region  Client

  public override void OnStartClient()
  {
    if (!isClientOnly) { return; }
    if (!hasAuthority) { return; }
    AuthorityOnUnitSpawned?.Invoke(this);
  }

  public override void OnStopClient()
  {
    if (isClientOnly) { return; }
    if (!hasAuthority) { return; }
    AuthorityOnUnitDespawned?.Invoke(this);
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
