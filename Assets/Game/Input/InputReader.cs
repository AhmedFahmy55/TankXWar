using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;


[CreateAssetMenu(fileName = "New player Input",menuName = "Input/New Input Asset")]
public class InputReader : ScriptableObject, IPlayerActions

{
    public event Action<bool> OnPlayreFire;

    public Vector2 Movement { get; private set; }
    public Vector2 MousePosition { get;private set; }

    private Controls controls;


    private void OnEnable()
    {
        if(controls == null) 
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Enable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            OnPlayreFire?.Invoke(true);
        }
        else if(context.canceled)
        {
            OnPlayreFire?.Invoke(false);

        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }
}
