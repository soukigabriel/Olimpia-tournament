using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Player { Player1, Player2 }

public class PlayerController : MonoBehaviour
{
    //public delegate void PlayerDelegate();
    //public event PlayerDelegate PlayerAttack;

    public Quaternion rightRotation;
    public Quaternion leftRotation;

    public Player thisPlayer;

    [Space]
    [Header("Behaviour Variables")]
    Transform otherPlayer;
    [SerializeField] Transform m_player;
    PlayerResources m_PlayerResources;

    [Space]
    [Header("Movement variables")]
    public Vector2 axis;
    public Vector2 movement;
    [SerializeField] private float smoothInputSpeed = 0.2f;
    private float smoothInputVelocity;
    [SerializeField] float jumpSpeed;
    public bool canJump;
    public LayerMask groundMask;
    [SerializeField] float rayLenght = 2f;
    public Vector3 raycastOffset = new Vector3(0, 0.775f, 0);
    public bool canMove;
    public bool isInCombo;
    Vector3 initialPosition;

    [Space]
    [Header("Animations variables")]
    [SerializeField] Animator m_animator;
    public bool facingRight = true;
    readonly int m_HashStateTime = Animator.StringToHash("StateTime");
    readonly int m_HashStateBasicAttack = Animator.StringToHash("BasicAttack");
    readonly int m_HashStateIsWalking = Animator.StringToHash("IsWalking");
    readonly int m_HashStateHorizontalAxis = Animator.StringToHash("HorizontalAxis");
    readonly int m_HashStateHorizontalAxisAbs = Animator.StringToHash("HorizontalAxisAbs");
    readonly int m_HashStatePositionDiference = Animator.StringToHash("PositionDiference");
    readonly int m_HashStateGrounded = Animator.StringToHash("Grounded");
    int m_HashParameterIsInCombo = Animator.StringToHash("IsInCombo");
    int m_HashParameterVerticalVelocity = Animator.StringToHash("VerticalSpeed");

    [Space]
    [Header("Audio")]
    [SerializeField] AudioClip[] attackAudio;
    [SerializeField] AudioClip[] hurtAudio;
    [SerializeField] AudioClip[] deathAudio;
    int attackAudioLength, hurtAudioLength, deathAudioLength;
    AudioSource m_AudioSource;

    public PlayerInput m_PlayerInput;

    public CharacterController m_CharacterController;
    private float ySpeed;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    public float jumpButtonGracePeriod;
    public float lastGroundedGraceTime;


    void GetOtherPlayer()
    {
        switch (thisPlayer)
        {
            case Player.Player1: otherPlayer = GameObject.FindGameObjectWithTag("Player2").transform;
                break;
            case Player.Player2: otherPlayer = GameObject.FindGameObjectWithTag("Player1").transform;
                break;
        }
    }

    private void OnDisable()
    {
        GameManager.OnInGame -= InitialSettings;
    }

    private void InitialSettings()
    {
        canJump = true;
        canMove = true;
        m_animator.SetTrigger("Revive");
        isInCombo = false;
        GetOtherPlayer();
        SetPositions();
    }

    private void Start()
    {
        initialPosition = this.transform.position;
        InitialSettings();
        GameManager.OnInGame += InitialSettings;
        attackAudioLength = attackAudio.Length;
        hurtAudioLength = hurtAudio.Length;
        deathAudioLength = deathAudio.Length;
        m_AudioSource = gameObject.GetComponent<AudioSource>();
        m_PlayerResources = gameObject.GetComponent<PlayerResources>();
    }

    private void FixedUpdate()
    {
        ySpeed += Physics.gravity.y * Time.fixedDeltaTime;
        if (m_CharacterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            canJump = true;
            ySpeed = -0.5f;
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                canJump = false;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            canJump = false;
        }
        m_animator.ResetTrigger(m_HashStateBasicAttack);
        m_animator.SetBool(m_HashParameterIsInCombo, isInCombo);
    }

    private void Update()
    {
        Move();

        m_animator.SetFloat(m_HashStateTime, Mathf.Repeat(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        m_animator.SetBool(m_HashStateGrounded, Time.time - lastGroundedTime <= lastGroundedGraceTime);
    }

    void SetPositions()
    {
        //Set where the character will be facing
        if(thisPlayer == Player.Player1)
        {
            facingRight = true;
        }
        if(thisPlayer == Player.Player2)
        {
            facingRight = false;
        }
        this.transform.position = initialPosition;
        SetFacing();
    }

    void OnBasicAttack()
    {
        if(GameManager.sharedInstance.CurrentGameState == GameState.inGame && m_CharacterController.isGrounded)
        {
            m_animator.SetTrigger(m_HashStateBasicAttack);
            canMove = false;
            isInCombo = true;
            m_animator.SetBool(m_HashParameterIsInCombo, isInCombo);



            //PlayRandomAttackAudio();
            //canMove = false;
        }
    }

    public void PlayRandomHurtAudio()
    {
        if(m_PlayerResources.CurrentHealth > 0)
        {
            int randomIndex = Random.Range(0, hurtAudioLength);
            m_AudioSource.PlayOneShot(hurtAudio[randomIndex]);
        }
        else
        {
            PlayRandomDeathAudio();
        }
    }

    public void ActivateRootMotion()
    {
        m_animator.applyRootMotion = true;
    }

    public void PlayRandomDeathAudio()
    {
        int randomIndex = Random.Range(0, deathAudioLength);
        m_AudioSource.PlayOneShot(deathAudio[randomIndex]);
    }

    public void PlayRandomAttackAudio()
    {
        int randomIndex = Random.Range(0, attackAudioLength);
        m_AudioSource.PlayOneShot(attackAudio[randomIndex]);
    }

    void SetCanMove()
    {
        canMove = true;
    }

    void OnJump()
    {
        if(canJump)
        {
            jumpButtonPressedTime = Time.time;
        }
    }

    void OnRun(InputValue value)
    {
        if(value.Get<float>() == 1f)
        {
            m_animator.SetBool("isRunning", true);
        }
        else
        {
            m_animator.SetBool("isRunning", false);
        }
    }

    void OnMove(InputValue movementValue)
    {
        axis = movementValue.Get<Vector2>();
    }


    void Move()
    {
        //if (GameManager.sharedInstance.CurrentGameState == GameState.inGame)
        //{
        //    movement.x = Mathf.SmoothDamp(movement.x, axis.x, ref smoothInputVelocity, smoothInputSpeed);
        //}
        movement = axis;
        SetAnimations();
    }

    private void OnAnimatorMove()
    {
        Vector3 velocity = m_animator.deltaPosition;
        velocity = AdjustVelocityToSlope(velocity);
        velocity.y += ySpeed * Time.deltaTime;
        velocity.z = 0f;
        m_CharacterController.Move(velocity);
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelovity = slopeRotation * velocity;

            if(adjustedVelovity.y < 0)
            {
                return adjustedVelovity;
            }
        }
        return velocity;
    }

    private void SetAnimations()
    {
        if (movement.x == 0)
        {
            m_animator.SetBool(m_HashStateIsWalking, false);
        }
        else
        {
            m_animator.SetBool(m_HashStateIsWalking, true);
        }
        if (otherPlayer.position.x > this.transform.position.x && !facingRight && m_CharacterController.isGrounded)
        {
            facingRight = true;
            SetFacing();
        }
        else if (otherPlayer.position.x < this.transform.position.x && facingRight && m_CharacterController.isGrounded)
        {
            facingRight = false;
            SetFacing();
        }
        m_animator.SetFloat(m_HashStateHorizontalAxis, movement.x, smoothInputSpeed, Time.deltaTime);
        m_animator.SetFloat(m_HashParameterVerticalVelocity, ySpeed);
        m_animator.SetFloat(m_HashStateHorizontalAxisAbs, Mathf.Abs(movement.x), smoothInputSpeed, Time.deltaTime);
        m_animator.SetFloat(m_HashStatePositionDiference, otherPlayer.position.x - this.transform.position.x);
    }

    void SetFacing()
    {
        if (facingRight)
        {
            m_player.rotation = rightRotation;
        }
        else
        {
            m_player.rotation = leftRotation;
        }
    }

}
