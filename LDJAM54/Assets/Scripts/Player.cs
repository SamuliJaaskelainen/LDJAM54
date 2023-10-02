using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public const int PLAYER_START_HEALTH = 20;
    [SerializeField] CharacterController characterController;
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] Transform head;
    [SerializeField] TextMeshProUGUI healthUi;
    [SerializeField] AnimationCurve barrelRollCurve;
    [SerializeField] Transform heartUI;
    [SerializeField] AnimationCurve heartCurve;
    [SerializeField] LayerMask attackLayers;
    [SerializeField] GameObject help;
    [SerializeField] GameObject helpHelp;
    [SerializeField] GameObject gameOver;
    static float mouseSensitivity = 0.15f;
    public float forwardSpeed;
    public float strafeSpeed;
    public float jumpPower;
    public float jumpPowerAdd;
    public float barrelRollSpeed;
    public float gravity;
    public float groundDrag;
    public float airDrag;
    public float attackRate;
    public float attackDistance;
    public float attackRadius;
    public float bounceForceUp;
    public float bounceForceForward;
    public static float health = PLAYER_START_HEALTH;
    public static int killCount;
    Vector3 velocity = Vector3.zero;
    float attackTimer;
    private bool isIdle = true;
    private SpriteAnimation[] animationArray;
    RaycastHit hit;
    float headRotationUp = 0.0f;
    float barrelRoll = 0.0f;
    bool doingBarrelRoll = false;
    float heartAnimValue = 0.0f;
    float footstepSoundTime = 0.0f;
    float screenshakeAmount = 0.0f;
    Vector3 screenshakeValue;
    bool wasGrounded;
    float jumpPressedTimer;
    bool jumpReleased = true;
    bool isDead = false;
    float deadTimer = 0.0f;
    float deadHeadValue = 0.0f;

    private void Awake()
    {
        Instance = this;
        animationArray = GetComponentsInChildren<SpriteAnimation>();
        help.SetActive(false);
        gameOver.SetActive(false);
    }

    void Update() {
        // Generic actions
        helpHelp.SetActive(killCount < 3);
        if (Input.GetKeyDown(KeyCode.F1))
        {
            help.SetActive(!help.activeSelf);
            Time.timeScale = help.activeSelf ? 0.0f : 1.0f;
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            mouseSensitivity += Time.unscaledDeltaTime * 0.1f;
        }
        else if (Input.GetKey(KeyCode.PageDown))
        {
            mouseSensitivity -= Time.unscaledDeltaTime * 0.1f;
        }
        mouseSensitivity = Mathf.Clamp(mouseSensitivity, 0.001f, 3.0f);

        if (Input.GetKeyDown(KeyCode.Escape))
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

        // Handle afterlife
        if(isDead)
        {
            deadHeadValue += Time.deltaTime * 150.0f;
            if(deadHeadValue > 95.0f)
            {
                foreach (SpriteAnimation animation in animationArray)
                {
                    animation.enabled = false;
                }
                deadHeadValue = 95.0f;
                GetComponentInChildren<TentacleShake>().enabled = false;
            }
            head.transform.localEulerAngles = new Vector3(0.0f, 0.0f, deadHeadValue);

            deadTimer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Mouse0) && deadTimer > 1.0f)
            {
                LevelManager.Instance.Lose();
            }

            return;
        }

        // Get player action data
        float forwardMovement = 0.0f;
        float rightMovement = 0.0f;
        bool jump = Input.GetKey(KeyCode.Space);
        bool bounce = Mouse.current.rightButton.isPressed;
        bool attack = Mouse.current.leftButton.isPressed;
        bool headCollision = Physics.Raycast(transform.position, Vector3.up, characterController.height, collisionLayers);
        Vector2 mouseDelta = new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y.value) * mouseSensitivity;

        if (Input.GetKey(KeyCode.W)) {
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
                jumpPressedTimer = 0.0f;
                jumpReleased = false;
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
                    Screenshake(10.0f);
                }
            }
        }

        if (jump)
        {
            if (!jumpReleased)
            {
                //Debug.Log("ADDD");
                velocity.y += jumpPowerAdd * Time.deltaTime;
                jumpPressedTimer += Time.deltaTime;
                if (jumpPressedTimer > 0.15f)
                {
                    jumpReleased = true;
                }
            }
        }
        else
        {
            jumpReleased = true;
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
        
        // If player is jumping, play jump animation
        // THIS IS CURRENTLY BUGGED, DELETING THIS FIXES BUG
        if (!characterController.isGrounded && !attack && !bounce && !doingBarrelRoll)
        {
            foreach (SpriteAnimation animation in animationArray)
            {
                animation.runAnimation(2);
            }
        }

        // Update attack
        if (attack && Time.time > attackTimer) {
            // get each sprite animation component and then change it to the attack animation
            foreach (SpriteAnimation animation in animationArray)
            {
                animation.runAnimation(1);
            }
            isIdle = false;
            //Debug.Log("Attack!");
            //Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.yellow, duration:1);
            attackTimer = Time.time + attackRate;
            PlayAttackSound();
            
            // cast a sphere starting slightly behind you so you can hit enemies that are close
            if (Physics.SphereCast(transform.position - (transform.forward * attackRadius), attackRadius, 
                    transform.forward, out hit, maxDistance:attackDistance, collisionLayers))
            {
                
                if(hit.transform.tag == "Enemy")
                {
                    hit.transform.SendMessage("Die");
                    health += 3;
                    PlayTentacleHitSound();
                }
            }
        }
        
        // reset the animation back to the idle animation after the attack animation is done
        if (attackTimer < Time.time && !isIdle) {
            // get each sprite animation component and then change it to the idle animation
            foreach (SpriteAnimation animation in animationArray)
            {
                animation.runAnimation(0);
            }
            isIdle = true;
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

        // Apply screenshake
        if(!wasGrounded && characterController.isGrounded)
        {
            Screenshake(10.0f);
        }
        screenshakeAmount -= Time.deltaTime * 100.0f;
        if(screenshakeAmount < 0.0f)
        {
            screenshakeAmount = 0.0f;
        }
        Vector3 screenshakeTarget = Random.insideUnitSphere;
        screenshakeValue = Vector3.RotateTowards(screenshakeValue, screenshakeTarget, Time.deltaTime * 50.0f, Time.deltaTime * 50.0f);
        head.localEulerAngles += screenshakeValue * screenshakeAmount;

        // Update HP
        health -= Time.deltaTime;
        healthUi.text = Mathf.CeilToInt(health).ToString();
        if(health <= 0)
        {
            gameOver.SetActive(true);
            isDead = true;
            PlayDeathSound();
        }

        // Animate HP
        heartAnimValue += Time.deltaTime * (health < 10 ? 6.0f : 4.0f);
        if(heartAnimValue > 1.0f)
        {
            heartAnimValue = 0.0f;
        }
        heartUI.localScale = Vector3.one * heartCurve.Evaluate(heartAnimValue);
        
        // If player is moving and on the ground, play footstep sound
        if (characterController.isGrounded && (velocity.x  != 0.0f || velocity.z != 0.0f))
        {
            PlayFootstepSound();
        }
        


        // Set values for next frame
        wasGrounded = characterController.isGrounded;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RoomEnd")
        {
            //Debug.Log("Hit room end! " + other.gameObject.GetInstanceID());
            LevelManager.Instance.SpawnNextRoom();
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
    
    private void PlayFootstepSound()    
    {
        if (Time.time - footstepSoundTime < CalculateFootstepFrequency())
        {
            return;
        }
        else
        {
            AudioManager.Instance.PlaySound(Random.Range(0,10), transform.position, 0.3f);
            footstepSoundTime = Time.time;
        }

    }

    private float CalculateFootstepFrequency()
    {
        // so we know how often to play the footstep sound

        return 1.0f / Mathf.Sqrt((velocity.x * velocity.x) + (velocity.z * velocity.z));
        
    }

    public void Hurt(int damage)
    {
        if(doingBarrelRoll)
        {
            return;
        }

        health -= damage;
        if(health < 0.0f)
        {
            health = 0.0f;
        }
        Screenshake(25.0f);
        PlayHurtSound();
    }
    
    private void PlayHurtSound()    
    {
        if (isDead) return;
        AudioManager.Instance.PlaySound(Random.Range(14,18), transform.position, 0.7f);

    }
    
    private void PlayTentacleHitSound() {
        AudioManager.Instance.PlaySound(Random.Range(10,14), transform.position, 0.9f);

    }

    private void PlayAttackSound()
    {
        AudioManager.Instance.PlaySound(Random.Range(18,22), transform.position, 0.5f);
    }
    
    private void PlayDeathSound()
    {
        // you need to add the vector3.right to correct the sound position
        AudioManager.Instance.PlaySound(65, transform.position + Vector3.right, 1f);
    }


    public void Screenshake(float strenght)
    {
        if(strenght > screenshakeAmount)
        {
            screenshakeAmount = strenght;
        }
    }
}
