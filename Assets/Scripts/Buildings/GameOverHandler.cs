using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> ClientHandleGameOver;
    private List<UnitBase> unitBases = new List<UnitBase>();
    #region Server
    public override void OnStartServer() {
        UnitBase.OnUnitbaseSpawned += HandleUnitBaseSpawned;
        UnitBase.OnUnitbaseDespawned += HandleUnitBaseDespawned;
    }

    public override void OnStopServer() {
        UnitBase.OnUnitbaseSpawned -= HandleUnitBaseSpawned;
        UnitBase.OnUnitbaseDespawned -= HandleUnitBaseDespawned;
    }

    private void HandleUnitBaseSpawned(UnitBase unitBase){
        unitBases.Add(unitBase);
    }

    private void HandleUnitBaseDespawned(UnitBase unitBase) {
        unitBases.Remove(unitBase);

        if (unitBases.Count == 1) {
            int playerID = unitBases[0].netIdentity.connectionToClient.connectionId;
            RpcHandleGameOver($"Player {playerID}");
        }
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcHandleGameOver(string winner) {
        ClientHandleGameOver?.Invoke(winner);
    }

    #endregion
}
