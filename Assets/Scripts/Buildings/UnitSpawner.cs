using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
  [SerializeField] private Unit unitPrefab = null;
  [SerializeField] private Transform spawnUnitPosition = null;

  [SerializeField] private Health health = null;

  [SerializeField] private TMP_Text remainingUnitsText = null;
  [SerializeField] private Image timedImage = null;
  [SerializeField] private int maxUnitQueue = 5;
  [SerializeField] private float spawnMoveRange = 7f;
  [SerializeField] private float unitSpawnduration = 5f;

  private float progressVelocity;
  private RTSPlayer player;

  [SyncVar(hook = nameof(onClientQuenedUnitUpdated))]
  private int queuedUnits;

  [SyncVar]
  private float unitTimer = 0;



  #region Server

  public override void OnStartServer()
  {
    timedImage.fillAmount = 0;
    health.ServerDieEvent += OnServerDie;
    player = connectionToClient.identity.GetComponent<RTSPlayer>();
  }

  public override void OnStopServer()
  {
    health.ServerDieEvent -= OnServerDie;
  }

  private void Update() {
    if (isServer) {
      serverUpdateSpawning();
    }
    if (isClient) {
      clientUpdateTimer();
    }
  }

  [Server]
  private void serverUpdateSpawning() {
      if (queuedUnits == 0) { return; }
      unitTimer += Time.deltaTime;
      if (unitTimer >= unitSpawnduration)
      {
        SpawnUnit();
        queuedUnits--;
        unitTimer = 0;
      }
  }
  private void OnServerDie()
  {
    NetworkServer.Destroy(gameObject);
  }

  [Command]
  private void CmdSpawnUnit()
  {
    if(queuedUnits == maxUnitQueue) { return; }
    if (player.GetResources() < unitPrefab.GetUnitCost()) {
      return;
    }
    queuedUnits ++;
    player.SetResources(player.GetResources() - unitPrefab.GetUnitCost());
  }

  [Server]
  private void SpawnUnit() {
    GameObject unit = Instantiate(
        unitPrefab.gameObject,
    spawnUnitPosition.position,
    spawnUnitPosition.transform.rotation);

    NetworkServer.Spawn(unit, connectionToClient);

    Vector3 randomPos = (spawnMoveRange * Random.insideUnitSphere);
    randomPos.y = spawnUnitPosition.position.y;

    unit.GetComponent<UnitMovement>().MoveTo(randomPos + spawnUnitPosition.position);
  }

  #endregion

  #region Client
  public void OnPointerClick(PointerEventData eventData)
  {
    if (eventData.button != PointerEventData.InputButton.Left) { return; }
    if (!hasAuthority) { return; }
    CmdSpawnUnit();
  }

  private void onClientQuenedUnitUpdated(int oldValue, int newValue)
  {
    remainingUnitsText.text = $"{newValue}";
  }

  private void clientUpdateTimer() {
    float progress = unitTimer / unitSpawnduration;
    if (progress <= timedImage.fillAmount) {
      timedImage.fillAmount = progress;
    } else {
      timedImage.fillAmount = Mathf.SmoothDamp(timedImage.fillAmount, progress, ref progressVelocity, 0.1f);
    }
  }
  #endregion
}
