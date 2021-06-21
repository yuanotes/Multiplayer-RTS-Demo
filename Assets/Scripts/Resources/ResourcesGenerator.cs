using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ResourcesGenerator : NetworkBehaviour
{
  [SerializeField] private float generateInterval = 2f;
  [SerializeField] private int generateAmount = 5;

  [SerializeField] private Health health = null;

  private RTSPlayer player;
  private float timer = 0f;

  public override void OnStartServer()
  {
    timer = generateInterval;
    player = connectionToClient.identity.GetComponent<RTSPlayer>();
    health.ServerDieEvent += onServerDie;
    GameOverHandler.ServerGameOverEvent += onServerGameOver;
  }

  public override void OnStopServer()
  {
    health.ServerDieEvent -= onServerDie;
    GameOverHandler.ServerGameOverEvent -= onServerGameOver;
  }

  [ServerCallback]
  // Update is called once per frame
  void Update()
  {
      timer -= Time.deltaTime;
      if (timer <=0) {
          player.SetResources(player.GetResources() + generateAmount);
          timer = generateInterval;
      }
  }

  private void onServerDie()
  {
    NetworkServer.Destroy(gameObject);
  }

  private void onServerGameOver()
  {
    enabled = false;
  }
}
