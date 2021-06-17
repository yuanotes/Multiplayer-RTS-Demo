using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{

  [SerializeField] private RectTransform dragSelection = null;
  [SerializeField] private LayerMask layerMask = new LayerMask();

  private Camera mainCamera;
  private Vector2 selectionStartPoint;

  private RTSPlayer player;

  public List<Unit> SelectedUnites { get; } = new List<Unit>();

  private void Start()
  {
    mainCamera = Camera.main;
    Unit.AuthorityUnitDespawnedEvent += onAuthorityDespawned;
  }

  private void OnDestroy() {
    Unit.AuthorityUnitDespawnedEvent -= onAuthorityDespawned;
  }

  private void Update()
  {
    if (player == null)
    {
      player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
      StartSelectionArea();
    }
    else if (Mouse.current.leftButton.wasReleasedThisFrame)
    {
      ClearSelectionArea();
    }
    else if (Mouse.current.leftButton.isPressed)
    {
      UpdateSelectionArea();
    }

  }

  private void StartSelectionArea()
  {
    if (!Keyboard.current.leftShiftKey.isPressed)
    {
      foreach (Unit selectedUnit in SelectedUnites)
      {
        selectedUnit.DeSelect();
      }
      SelectedUnites.Clear();
    }

    dragSelection.gameObject.SetActive(true);

    selectionStartPoint = Mouse.current.position.ReadValue();
    UpdateSelectionArea();
  }

  private void UpdateSelectionArea()
  {
    Vector2 currentPoint = Mouse.current.position.ReadValue();

    float width = currentPoint.x - selectionStartPoint.x;
    float height = currentPoint.y - selectionStartPoint.y;

    dragSelection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
    dragSelection.anchoredPosition = selectionStartPoint + new Vector2(width / 2, height / 2);
  }

  private void ClearSelectionArea()
  {

    if (dragSelection.rect.size.magnitude == 0)
    {
      Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
      if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
      {
        return;
      }

      if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

      if (!unit.hasAuthority) { return; }

      SelectedUnites.Add(unit);

      foreach (Unit selectedUnit in SelectedUnites)
      {
        selectedUnit.Select();
      }
    }
    else
    {
      dragSelection.gameObject.SetActive(false);
      float width = dragSelection.rect.width;
      float height = dragSelection.rect.height;
      float minX = dragSelection.anchoredPosition.x - width / 2;
      float minY = dragSelection.anchoredPosition.y - height / 2;
      float maxX = minX + width;
      float maxY = minY + height;

      foreach (Unit unit in player.GetMyUnits())
      {
        if (SelectedUnites.Contains(unit))
        {
          continue;
        }
        Vector3 unitPoint = mainCamera.WorldToScreenPoint(unit.transform.position);
        if (unitPoint.x > minX &&
        unitPoint.x < maxX &&
        unitPoint.y > minY &&
        unitPoint.y < maxY)
        {
          SelectedUnites.Add(unit);
          unit.Select();
        }
      }
    }
  }

  private void onAuthorityDespawned(Unit unit) {
    SelectedUnites.Remove(unit);
  }

}
