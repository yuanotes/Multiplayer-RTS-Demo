using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerGameOverEvent;
    public static event Action<string> ClientGameOverEvent;
    private List<UnitBase> unitBases = new List<UnitBase>();
    #region Server
    public override void OnStartServer()
    {
        UnitBase.UnitbaseSpawnedEvent += OnServerUnitbaseSpawned;
        UnitBase.UnitbaseDespawnedEvent += OnServerUnitBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.UnitbaseSpawnedEvent -= OnServerUnitbaseSpawned;
        UnitBase.UnitbaseDespawnedEvent -= OnServerUnitBaseDespawned;
    }

    [Server]
    private void OnServerUnitbaseSpawned(UnitBase unitBase)
    {
        unitBases.Add(unitBase);
    }

    [Server]
    private void OnServerUnitBaseDespawned(UnitBase unitBase)
    {
        unitBases.Remove(unitBase);

        if (unitBases.Count != 1) return;

        int playerID = unitBases[0].connectionToClient.connectionId;

        RpcHandleGameOver($"Player {playerID}");

        ServerGameOverEvent?.Invoke();
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcHandleGameOver(string winner)
    {
        ClientGameOverEvent?.Invoke(winner);
    }

    #endregion
}
