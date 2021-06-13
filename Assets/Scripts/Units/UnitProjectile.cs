using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
  [SerializeField] Rigidbody rb = null;
  [SerializeField] float launchForce = 20f;
  [SerializeField] float destroyAfterSeconds = 5f;
  // Start is called before the first frame update
  void Start()
  {
    rb.velocity = transform.forward * launchForce;
  }

  public override void OnStartServer()
  {
    Invoke(nameof(DestroySelf), destroyAfterSeconds);
  }

  private void DestroySelf()
  {
    NetworkServer.Destroy(gameObject);
  }
}
