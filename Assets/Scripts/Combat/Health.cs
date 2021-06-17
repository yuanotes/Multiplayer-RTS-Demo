using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Health : NetworkBehaviour
{
  [SerializeField] private int maxHealth = 100;

  [SyncVar(hook = nameof(onHealthUpdated))]
  private int currentHealth;

  public event Action ServerDieEvent;

  public event Action<int, int> ClientHealthUpdatedEvent;

  #region Server

  public override void OnStartServer()
  {
    currentHealth = maxHealth;
    UnitBase.ServerPlayerDieEvent += onServerPlayerDie;
  }

  public override void OnStopServer()
  {
    UnitBase.ServerPlayerDieEvent -= onServerPlayerDie;
  }

  [Server]
  private void onServerPlayerDie(int playerDieID)
  {
    if (connectionToClient.connectionId != playerDieID)
    {
      return;
    }
    DealDamage(currentHealth);
  }

  [Server]
  public void DealDamage(int damage)
  {
    if (currentHealth == 0)
    {
      return;
    }
    currentHealth = Math.Max(currentHealth - damage, 0);

    if (currentHealth > 0)
    {
      return;
    }

    ServerDieEvent?.Invoke();
  }
  #endregion

  #region Client
  void onHealthUpdated(int oldValue, int newValue)
  {
    ClientHealthUpdatedEvent?.Invoke(newValue, maxHealth);
  }

  #endregion
}
