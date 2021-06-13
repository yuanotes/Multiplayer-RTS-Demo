using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
  [SerializeField] private NavMeshAgent agent = null;
  [SerializeField] private Targeter targeter = null;
  [SerializeField] private float chaseRange = 10.0f;

  #region Server
  [ServerCallback]
  private void Update()
  {
    GameObject target = targeter.GetTarget().gameObject;

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



  [Command]
  public void CmdMove(Vector3 position)
  {
    targeter.ClearTarget();
    if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
    {
      return;
    }
    agent.SetDestination(hit.position);
  }

  #endregion
}

