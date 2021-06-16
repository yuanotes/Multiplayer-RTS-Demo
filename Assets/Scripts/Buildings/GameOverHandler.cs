using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
  public static event Action<string> ClientHandleGameOver;
  private List<UnitBase> unitBases = new List<UnitBase>();
  #region Server
  public override void OnStartServer()
  {
    UnitBase.OnUnitbaseSpawned += ServerHandleUnitBaseSpawned;
    UnitBase.OnUnitbaseDespawned += ServerHandleUnitBaseDespawned;
  }

  public override void OnStopServer()
  {
    UnitBase.OnUnitbaseSpawned -= ServerHandleUnitBaseSpawned;
    UnitBase.OnUnitbaseDespawned -= ServerHandleUnitBaseDespawned;
  }

  [Server]
  private void ServerHandleUnitBaseSpawned(UnitBase unitBase)
  {
    unitBases.Add(unitBase);
  }

  [Server]
  private void ServerHandleUnitBaseDespawned(UnitBase unitBase)
  {
    unitBases.Remove(unitBase);

    if (unitBases.Count == 1)
    {
      int playerID = unitBases[0].connectionToClient.connectionId;
      RpcHandleGameOver($"Player {playerID}");
    }
  }

  #endregion

  #region Client
  [ClientRpc]
  private void RpcHandleGameOver(string winner)
  {
    ClientHandleGameOver?.Invoke(winner);
  }

  #endregion
}
