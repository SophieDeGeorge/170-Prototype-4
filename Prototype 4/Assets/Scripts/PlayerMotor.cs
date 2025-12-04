using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    // movement
    public float speed = 4f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    // crouch
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 2.5f;
    public float crouchTime = 0.2f;   // a bit faster feels nicer, but tweak

    private bool crouching = false;
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    
    // sprint
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    // private bool sprinting = false;
    
    private bool sprintHeld = false;
    private bool crouchHeld = false;

    private Vector2 moveInput = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        controller.height = standHeight;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        HandleCrouchLerp();


        speed = sprintHeld ? sprintSpeed : walkSpeed;

        if (crouching)
        {
            speed = crouchSpeed;                   // slower while crouched
        }
        else
        {
            speed = sprintHeld ? sprintSpeed : walkSpeed;
        }
    }

    void HandleCrouchLerp()
    {
        if (!lerpCrouch) return;

        crouchTimer += Time.deltaTime;
        float p = crouchTimer / crouchTime;
        p *= p; // ease-in

        float targetHeight = crouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, p);

        if (p >= 1f)
        {
            lerpCrouch = false;
            crouchTimer = 0f;
        }
        }

    public void ProcessMove(Vector2 input)
    {
        moveInput = input;

        Vector3 moveDir = Vector3.zero;
        moveDir.x = input.x;
        moveDir.z = input.y;

        controller.Move(transform.TransformDirection(moveDir) * speed * Time.deltaTime);

        // gravity
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    public void SetCrouch(bool isHeld)
    {
        if (crouchHeld == isHeld) return; // no change

        crouchHeld = isHeld;
        crouching = isHeld;
    
        if (crouching)
        {
            sprintHeld = false;          // stop sprinting when crouch starts
        }

        crouchTimer = 0f;
        lerpCrouch = true;
    }

    public void SetSprint(bool isHeld)
    {
        sprintHeld = isHeld;
    }

}