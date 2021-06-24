using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour { //DevSkim: ignore DS184626
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float screenBorder = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    [SerializeField] private Vector2 screenYLimits = Vector2.zero;

    private Controls controls;

    private Vector2 previousInput;
    private float previousScroll;

    public override void OnStartAuthority() {
        playerCameraTransform.gameObject.SetActive(true);
        controls = new Controls();

        controls.Player.MoveCamera.performed += onSetPreviousInput;
        controls.Player.MoveCamera.canceled += onSetPreviousInput;
        controls.Player.ZoomCamera.performed += onSetPreviousScale;
        controls.Player.ZoomCamera.canceled += onSetPreviousScale;

        controls.Enable();
    }

    [ClientCallback]
    private void Update() {
        if (!hasAuthority || !Application.isFocused) {
            return;
        }
        updateCameraPosition();
    }

    private void updateCameraPosition() {
        Vector3 pos = playerCameraTransform.position;
        if (previousScroll != 0 ) {
            float delta;
            if (previousScroll < 0 ) {
                delta = -1 * scrollSpeed * Time.deltaTime;
            } else {
                delta = 1 * scrollSpeed * Time.deltaTime;
            }
            pos.y = Mathf.Clamp(pos.y + delta, screenYLimits.x, screenYLimits.y);

        } else if (previousInput == Vector2.zero) {
            Vector3 cursorMovement = Vector3.zero;
            Vector2 cursorPosition = Mouse.current.position.ReadValue();
            if (cursorPosition.y >= Screen.height - screenBorder) {
                cursorMovement.z += 1;
            } else if (cursorPosition.y <= screenBorder) {
                cursorMovement.z -= 1;
            } else if (cursorPosition.x >= Screen.width - screenBorder) {
                cursorMovement.x += 1;
            } else if (cursorPosition.x <= screenBorder) {
                cursorMovement.x -= 1;
            }
            pos += cursorMovement.normalized * moveSpeed * Time.deltaTime;
        } else {
            pos += new Vector3(previousInput.x, 0f, previousInput.y)  * moveSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void onSetPreviousInput(InputAction.CallbackContext ctx) {
        previousInput = ctx.ReadValue<Vector2>();
    }
    private void onSetPreviousScale(InputAction.CallbackContext ctx) {
        previousScroll = ctx.ReadValue<float>();
    }
 }
