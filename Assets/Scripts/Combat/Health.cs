using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
  [SerializeField] private int maxHealth = 100;

  [SyncVar(hook = nameof(onHealthUpdated))]
  private int currentHealth;

  public event Action ServerOnDie;

  public event Action<int, int> ClientOnHealthUpdated;

  #region Server

  public override void OnStartServer()
  {
      currentHealth = maxHealth;
  }

  [Server]
  public void DealDamage(int damage) {
      if (currentHealth == 0) {
          return;
      }
      currentHealth = Math.Max(currentHealth - damage, 0);

      if (currentHealth > 0) {
          return;
      }

      ServerOnDie?.Invoke();

      Debug.Log("Object die!");
  }
  #endregion

  #region Client
  void onHealthUpdated(int oldValue, int newValue) {
      ClientOnHealthUpdated?.Invoke(newValue, maxHealth);
  }

  #endregion
}
