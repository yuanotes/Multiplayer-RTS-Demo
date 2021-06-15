using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
  [SerializeField] private GameObject projectilePrefab = null;
  [SerializeField] private Transform projectileSpawnPoint = null;
  [SerializeField] private float firingDistance = 11f;
  [SerializeField] private float firingRate = 1f;

  [SerializeField] private float rotateSpeed = 180f;


  [SerializeField] Targeter targeter = null;

  private float lastFiringTime = 0f;

  [ServerCallback]
  void Update()
  {
    Targetable target = targeter.GetTarget();
    if (target == null)
    {
      return;
    }
    if ((target.transform.position - transform.position).sqrMagnitude > firingDistance * firingDistance)
    {
      return;
    }
    Quaternion rotationTo = Quaternion.LookRotation(target.transform.position - transform.position);

    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationTo, rotateSpeed * Time.deltaTime);

    if (Time.time > (1 / firingRate) + lastFiringTime)
    {
      Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);
      GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
      NetworkServer.Spawn(projectile, connectionToClient);
      lastFiringTime = Time.time;
    }
  }
}
