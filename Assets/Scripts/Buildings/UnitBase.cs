using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<int> ServerOnPlayerDieEvent;
    public static event Action<UnitBase> UnitbaseSpawnedEvent;
    public static event Action<UnitBase> UnitbaseDespawnedEvent;

    #region Server
    public override void OnStartServer() {
        UnitbaseSpawnedEvent?.Invoke(this);
        health.ServerDieEvent += OnServerDie;
    }

    public override void OnStopServer() {
        UnitbaseDespawnedEvent?.Invoke(this);
        health.ServerDieEvent -= OnServerDie;
    }

    [Server]
    private void OnServerDie() {
        NetworkServer.Destroy(gameObject);
        ServerOnPlayerDieEvent?.Invoke(connectionToClient.connectionId);
    }
    #endregion

    #region Client

    #endregion
}
