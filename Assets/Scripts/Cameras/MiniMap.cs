using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
using UnityEngine.InputSystem;

public class MiniMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
  [SerializeField] private RectTransform miniMapRect = null;
  [SerializeField] private float cameraScale = 10f;
  [SerializeField] private float offsetZ = -5f;

  private Transform playerCameraTransform;


  private void Update()
  {
    if (playerCameraTransform != null) { return; }
    if (NetworkClient.connection.identity == null) { return; }
    if (playerCameraTransform == null)
    {
      playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }
  }
  public void MoveCamera()
  {
    Vector2 mousePoint = Mouse.current.position.ReadValue();
    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
        miniMapRect,
        mousePoint,
        null,
        out Vector2 localPoint))
    {
      return;
    }
    Vector2 lerp = new Vector2(
        (localPoint.x - miniMapRect.rect.x) / miniMapRect.rect.width,
        (localPoint.y - miniMapRect.rect.y) / miniMapRect.rect.height);

    Vector3 newCameraPos = new Vector3(
        Mathf.Lerp(-cameraScale, cameraScale, lerp.x),
        playerCameraTransform.position.y,
        Mathf.Lerp(-cameraScale, cameraScale, lerp.y));

    playerCameraTransform.position = newCameraPos + new Vector3(0f, 0f, offsetZ);
  }

  public void OnDrag(PointerEventData eventData)
  {
    MoveCamera();
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    MoveCamera();
  }
}
