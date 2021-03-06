using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
  [SerializeField] private NavMeshAgent agent = null;
  [SerializeField] private Targeter targeter = null;
  [SerializeField] private float chaseRange = 10.0f;

  #region Server
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
    agent.ResetPath();
  }

  [ServerCallback]
  private void Update()
  {
    Targetable target = targeter.GetTarget();

    if (target != null)
    {
      if ((target.transform.position - agent.transform.position).sqrMagnitude > chaseRange * chaseRange)
      {
        agent.SetDestination(target.transform.position);
      }
      else
      {
        agent.ResetPath();
      }
    }
    else
    {
      if (!agent.hasPath) return;
      if (agent.remainingDistance > agent.stoppingDistance)
      {
        return;
      }
      agent.ResetPath();
    }
  }

  [Server]
  public void MoveTo(Vector3 position) {
    targeter.ClearTarget();
    if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
    {
      return;
    }
    agent.SetDestination(hit.position);
  }

  [Command]
  public void CmdMove(Vector3 position)
  {
    MoveTo(position);
  }

  #endregion
}

