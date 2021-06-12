using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
  [SerializeField] private GameObject target;

  #region  Server
  [Command]
  public void CmdSetTarget(GameObject targetObject)
  {
    if (!targetObject.TryGetComponent<Targetable>(out Targetable newTarget))
    {
      return;
    }
    target = newTarget.gameObject;
  }

  [Server]
  public void ClearTarget() {
    target = null;
  }

  #endregion
}
