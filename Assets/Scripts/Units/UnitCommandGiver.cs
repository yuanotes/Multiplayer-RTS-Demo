using UnityEngine;
using UnityEngine.InputSystem;

class UnitCommandGiver : MonoBehaviour
{
  [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
  [SerializeField] private LayerMask layerMask = new LayerMask();

  private Camera mainCamera;

  private void Start()
  {
    mainCamera = Camera.main;
    GameOverHandler.ClientGameOverEvent += onClientGameOver;
  }

  private void OnDestroy() {
    GameOverHandler.ClientGameOverEvent -= onClientGameOver;
  }

  private void Update()
  {
    if (!Mouse.current.rightButton.wasPressedThisFrame)
    {
      return;
    }
    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
    {
      return;
    }
    if (hit.collider.TryGetComponent<Targetable>(out Targetable targetable)) {
      if (targetable.hasAuthority) {
        TryMove(hit.point);
        return;
      }

      TryTarget(targetable.gameObject);
      return;
    }

    TryMove(hit.point);
  }

  private void TryMove(Vector3 point)
  {
    foreach (Unit unit in unitSelectionHandler.SelectedUnites)
    {
      unit.GetUnitMovement().CmdMove(point);
    }
  }

  private void TryTarget(GameObject gameObject) {
    foreach (Unit unit in unitSelectionHandler.SelectedUnites)
    {
      unit.GetTargeter().CmdSetTarget(gameObject);
    }
  }

  private void onClientGameOver(string winner) {
    enabled = false;
  }
}
