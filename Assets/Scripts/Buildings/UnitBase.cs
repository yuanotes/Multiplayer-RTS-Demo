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
        OnUnitbaseSpawned?.Invoke(this);
        health.ServerOnDie += OnHandleServerDie;
    }

    public override void OnStopServer() {
        OnUnitbaseDespawned?.Invoke(this);
        health.ServerOnDie -= OnHandleServerDie;
    }

    private void OnHandleServerDie() {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client

    #endregion
}
