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

        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
    }

    void FixedUpdate(){
        //tells playermotor to move using the value from movement action 
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }
     void LateUpdate(){
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    // Update is called once per frame
    // void Update()
    // {
        
    // }
    private void OnEnable(){
        onFoot.Enable();
    }
    private void onDisable(){
        onFoot.Disable();
    }
}
