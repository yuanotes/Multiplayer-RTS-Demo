using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> OnUnitbaseSpawned;
    public static event Action<UnitBase> OnUnitbaseDespawned;

    #region Server
    public override void OnStartServer() {
        health.ServerOnDie += OnHandleServerDie;
        OnUnitbaseSpawned?.Invoke(this);
    }

    public override void OnStopServer() {
        health.ServerOnDie -= OnHandleServerDie;
        OnUnitbaseDespawned?.Invoke(this);
    }

    private void OnHandleServerDie() {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client

    #endregion
}
