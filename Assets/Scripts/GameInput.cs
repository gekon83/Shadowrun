using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameInput : MonoBehaviour {
    /*
    public event EventHandler OnInteractAction;
    public event EventHandler OnAlternateInteractAction;
    */
    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //playerInputActions.Player.Interact.performed += Interact_performed;
        //playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
    }
    /*
    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnAlternateInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    */
    public Vector2 GetMovementVectorNotmalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
    public bool Jump() {
        if (playerInputActions.Player.Jump.triggered) {
            return true;
        }
        return false;
    }
}
