using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameOverHandler : NetworkBehaviour
{
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
            Debug.Log("Game Over!");
        }
    }

    #endregion

    #region Client

    #endregion
}
