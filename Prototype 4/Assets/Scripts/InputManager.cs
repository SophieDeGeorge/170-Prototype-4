using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInput PlayerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerInput = new PlayerInput();
        onFoot = PlayerInput.OnFoot;
         
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => motor.Jump();

        // onFoot.Crouch.performed += ctx => motor.Crouch();
        // onFoot.Sprint.performed += ctx => motor.Sprint();
        // HOLD CROUCH
        onFoot.Crouch.performed += ctx => motor.SetCrouch(true);
        onFoot.Crouch.canceled  += ctx => motor.SetCrouch(false);

        //  HOLD SPRINT
        onFoot.Sprint.performed += ctx => motor.SetSprint(true);
        onFoot.Sprint.canceled  += ctx => motor.SetSprint(false);
    }

    void FixedUpdate(){
        //tells playermotor to move using the value from movement action 
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
     void LateUpdate(){
        //mouse look
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
 
    void OnEnable(){
        // make sure input is initialized before enabling
        if (PlayerInput == null)
        {
            PlayerInput = new PlayerInput();
            onFoot = PlayerInput.OnFoot;
        }

        onFoot.Enable();
    }
    void onDisable(){
        onFoot.Disable();
    }
}
