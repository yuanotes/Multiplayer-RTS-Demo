using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour { //DevSkim: ignore DS184626
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorder = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Controls controls;

    private Vector2 previousInput;

    public override void OnStartAuthority() {
        playerCameraTransform.gameObject.SetActive(true);
        controls = new Controls();

        controls.Player.MoveCamera.performed += onSetPreviousInput;
        controls.Player.MoveCamera.canceled += onSetPreviousInput;

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
        if (previousInput == Vector2.zero) {
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
            pos += cursorMovement.normalized * speed * Time.deltaTime;
        } else {
            pos += new Vector3(previousInput.x, 0f, previousInput.y)  * speed * Time.deltaTime;
        }
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void onSetPreviousInput(InputAction.CallbackContext ctx) {
        previousInput = ctx.ReadValue<Vector2>();

    }
 }
