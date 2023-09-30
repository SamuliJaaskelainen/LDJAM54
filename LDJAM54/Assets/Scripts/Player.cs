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
    [SerializeField] AnimationCurve barrelRollCurve;
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
    Vector3 velocity = Vector3.zero;
    float attackTimer;
    RaycastHit hit;
    float headRotationUp = 0.0f;
    float barrelRoll = 0.0f;
    bool doingBarrelRoll = false;

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

        // Get player action data
        float forwardMovement = 0.0f;
        float rightMovement = 0.0f;
        bool jump = Input.GetKey(KeyCode.Space);
        bool bounce = Mouse.current.rightButton.isPressed;
        bool attack = Mouse.current.leftButton.isPressed;
        bool headCollision = Physics.Raycast(transform.position, Vector3.up, characterController.height, collisionLayers);
        Vector2 mouseDelta = new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y.value) * mouseSensitivity;

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

        // Y movement
        if(characterController.isGrounded)
        {
            if (jump && !headCollision)
            {
                velocity.y = jumpPower;
            }
            else if (bounce && !headCollision)
            {
                velocity.y = bounceForceUp;
                forwardMovement = bounceForceForward;
                doingBarrelRoll = true;
            }
        }
        else
        {
            if (bounce && !headCollision)
            {
                doingBarrelRoll = true;
            }

            velocity.y += gravity * Time.deltaTime;

            if (headCollision)
            {
                if (velocity.y > 0.0f)
                {
                    velocity.y = 0.0f;
                }
            }
        }

        // XZ movement
        forwardMovement *= forwardSpeed * Time.deltaTime;
        rightMovement *= strafeSpeed * Time.deltaTime;
        velocity += new Vector3(rightMovement, 0.0f, forwardMovement);
        float velocityMultiplier = Mathf.Pow(characterController.isGrounded ? groundDrag : airDrag, Time.deltaTime * 60.0f);
        if (velocity.x * velocity.x + velocity.z * velocity.z < 0.01f)
        {
            velocityMultiplier = 0.0f;
        }
        velocity.x *= velocityMultiplier;
        velocity.z *= velocityMultiplier;

        // Apply movement
        characterController.Move(transform.TransformDirection(velocity * Time.deltaTime));

        // Update attack
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

        // Rotate character
        transform.Rotate(Vector3.up, mouseDelta.x, Space.World);

        // Rotate camera
        headRotationUp -= mouseDelta.y;
        headRotationUp = Mathf.Clamp(headRotationUp, -90.0f, 90.0f);
        float barrelRollRotation = 0.0f;
        if(doingBarrelRoll)
        {
            barrelRoll += Time.deltaTime * barrelRollSpeed;
            if (barrelRoll > 1.0f)
            {
                barrelRoll = 0.0f;
                doingBarrelRoll = false;
            }
            barrelRollRotation = 360.0f * barrelRollCurve.Evaluate(barrelRoll);
        }
        head.localEulerAngles = new Vector3(headRotationUp + barrelRollRotation, 0.0f, 0.0f);

        // Update HP
        health -= Time.deltaTime;
        healthUi.text = Mathf.CeilToInt(health).ToString();
        if(health <= 0)
        {
            LevelManager.Instance.Lose();
        }
    }
}
