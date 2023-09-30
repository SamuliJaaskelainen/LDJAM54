using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Player : MonoBehaviour
{
    public const int PLAYER_START_HEALTH = 9999;
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Transform head;
    [SerializeField] TextMeshProUGUI healthUi;
    public float mouseSensitivity = 1.0f;
    public float forwardSpeed;
    public float strafeSpeed;
    public float jumpPower;
    public float barrelRollSpeed;
    public float gravity;
    public float groundDrag;
    public float airDrag;
    public float attackRate;
    public float attackDistance;
    public float bounceForceUp;
    public float bounceForceForward;
    public static float health = PLAYER_START_HEALTH;
    float upVelocity = 0.0f;
    Vector2 xzVelocity = Vector2.zero;
    float attackTimer;
    RaycastHit hit;
    float headRotationUp = 0.0f;
    float barrelRoll = 0.0f;
    bool doingBarrerlRoll = false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Cursor.visible)
        {
            Cursor.visible = false;
        }

        float forwardMovement = 0.0f;
        float rightMovement = 0.0f;
        bool jump = Input.GetKey(KeyCode.Space);
        bool bounce = Mouse.current.rightButton.isPressed;
        bool attack = Mouse.current.leftButton.isPressed;
        bool headCollision = Physics.Raycast(transform.position, Vector3.up, characterController.height, collisionLayers);

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

        if (characterController.isGrounded && jump && !headCollision)
        {
            upVelocity = jumpPower;
        }
        else if (characterController.isGrounded && bounce && !headCollision)
        {
            upVelocity = bounceForceUp;
            forwardMovement = bounceForceForward;
            doingBarrerlRoll = true;
        }

        if (!characterController.isGrounded)
        {
            upVelocity += gravity;
        }

        if(headCollision)
        {
            if (upVelocity > 0.0f)
            {
                upVelocity = 0.0f;
            }
        }

        xzVelocity += new Vector2(rightMovement, forwardMovement);

        float multiplier = Mathf.Pow(characterController.isGrounded ? groundDrag : airDrag, Time.deltaTime * 60.0f);
        if (xzVelocity.x * xzVelocity.x + xzVelocity.y * xzVelocity.y < 0.1f)
        {
            multiplier = 0.0f;
        }
        xzVelocity *= multiplier;

        Vector3 movement = transform.TransformDirection(new Vector3(xzVelocity.x, upVelocity, xzVelocity.y) * Time.deltaTime);

        characterController.Move(movement);

        if (attack && Time.time > attackTimer)
        {
            Debug.Log("Attack!");
            attackTimer = Time.time + attackRate;
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackDistance, collisionLayers))
            {
                if(hit.transform.tag == "Enemy")
                {
                    hit.transform.SendMessage("Die");
                    health += 5;
                }
            }
        }

        Vector2 rotation = new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y.value) * mouseSensitivity;
        transform.Rotate(Vector3.up, rotation.x, Space.World);

        headRotationUp -= rotation.y;
        headRotationUp = Mathf.Clamp(headRotationUp, -90.0f, 90.0f);
        if(doingBarrerlRoll)
        {
            barrelRoll += Time.deltaTime * barrelRollSpeed;
            if(barrelRoll > 360.0f)
            {
                barrelRoll = 0.0f;
                doingBarrerlRoll = false;
            }
        }
        head.localEulerAngles = new Vector3(headRotationUp + barrelRoll, 0.0f, 0.0f);

        health -= Time.deltaTime;
        healthUi.text = Mathf.CeilToInt(health).ToString();
        
        if(health <= 0)
        {
            LevelManager.Instance.Lose();
        }
    }
}
