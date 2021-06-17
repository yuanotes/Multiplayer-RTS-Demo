using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
  private Targetable target;

  public Targetable GetTarget()
  {
    return target;
  }

  #region  Server
  public override void OnStartServer()
  {
    GameOverHandler.ServerGameOverEvent += onServerGameOver;
  }

  public override void OnStopServer()
  {
    GameOverHandler.ServerGameOverEvent -= onServerGameOver;

  }
  [Server]
  private void onServerGameOver() {
    ClearTarget();
  }

  [Command]
  public void CmdSetTarget(GameObject targetObject)
  {
    if (!targetObject.TryGetComponent<Targetable>(out Targetable newTarget))
    {
      return;
    }
    target = newTarget;
  }

  [Server]
  public void ClearTarget()
  {
    target = null;
  }

  #endregion
}
