using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] Transform head;
    public float mouseSensitivity = 1.0f;
    public float forwardSpeed;
    public float strafeSpeed;
    Vector3 lastMousePosition = Vector3.zero;

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

        Vector3 movement = new Vector3(rightMovement, 0.0f, forwardMovement) * Time.deltaTime;
        characterController.Move(transform.TransformDirection(movement));

        Vector2 rotation = new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y.value) * mouseSensitivity;
        transform.Rotate(Vector3.up, rotation.x, Space.World);
        head.Rotate(Vector3.right, -rotation.y, Space.Self);
    }
}
