using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Transform head;
    public float mouseSensitivity = 1.0f;
    public float forwardSpeed;
    public float strafeSpeed;
    public float jumpPower;
    public float gravity;
    float upVelocity = 0.0f;
    RaycastHit hit;

    void Start()
    {
        
    }

    void Update()
    {
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(Cursor.visible)
        {
            Cursor.visible = false;
        }

        float forwardMovement = 0.0f;
        float rightMovement = 0.0f;
        bool jump = Input.GetKey(KeyCode.Space);
        bool bounce = Mouse.current.rightButton.isPressed;
        bool attack = Mouse.current.leftButton.isPressed;

        if (Input.GetKey(KeyCode.W)) 
        {
            forwardMovement = 1.0f;        
        }
        else if (Input.GetKey(KeyCode.S))
        {
            forwardMovement = -1.0f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rightMovement = 1.0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rightMovement = -1.0f;
        }

        forwardMovement *= forwardSpeed;
        rightMovement *= strafeSpeed;

        if(characterController.isGrounded && jump)
        {
            upVelocity = jumpPower;
        }

        if(!characterController.isGrounded)
        {
            upVelocity += gravity;
        }

        if(Physics.Raycast(transform.position, Vector3.up, characterController.height, collisionLayers))
        {
            if(upVelocity > 0.0f)
            {
                upVelocity = 0.0f;
            }
        }


        Vector3 movement = transform.TransformDirection(new Vector3(rightMovement, upVelocity, forwardMovement) * Time.deltaTime);


        characterController.Move(movement);

        Vector2 rotation = new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y.value) * mouseSensitivity;
        transform.Rotate(Vector3.up, rotation.x, Space.World);
        head.Rotate(Vector3.right, -rotation.y, Space.Self);
    }
}
