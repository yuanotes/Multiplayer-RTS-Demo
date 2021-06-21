using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  [SerializeField] private GameObject buildingPreviewInstance = null;
  [SerializeField] private Renderer buildingPreviewInstanceRender = null;
  [SerializeField] private Building building = null;
  [SerializeField] private Image icon  = null;
  [SerializeField] private LayerMask layerMask = new LayerMask();
  private Camera mainCamera;
  private RTSPlayer player;

  private void Start()
  {
    mainCamera = Camera.main;
    icon.sprite = building.GetIcon();
  }

  private void Update()
  {
    if (player == null)
    {
      player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }
    updateBuildingPreview();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if (eventData.button != PointerEventData.InputButton.Left) { return; }
    buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
    buildingPreviewInstanceRender = buildingPreviewInstance.GetComponentInChildren<Renderer>();
    buildingPreviewInstance.SetActive(false);
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    if (buildingPreviewInstance == null) return;
    if (eventData.button != PointerEventData.InputButton.Left) { return; }
    Vector2 point = Mouse.current.position.ReadValue();
    Ray ray = mainCamera.ScreenPointToRay(point);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
    {
        player.CmdTryPlaceBuilding(building.GetId(), hit.point);
    }
    Destroy(buildingPreviewInstance);
  }

  private void updateBuildingPreview()
  {
    if (buildingPreviewInstance == null) { return; }
    Vector2 point = Mouse.current.position.ReadValue();
    Ray ray = mainCamera.ScreenPointToRay(point);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
    {
      if (!buildingPreviewInstance.activeSelf)
      {
        buildingPreviewInstance.SetActive(true);
      }
      buildingPreviewInstance.transform.position = hit.point;
    }
  }
}
