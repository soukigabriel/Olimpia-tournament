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
    Vector2 axis;
    private float m_HorizontalSpeed;
    private float m_VerticalSpeed;
    [SerializeField] private float smoothInputSpeed = 0.2f;
    private float smoothInputVelocity;
    [SerializeField] float jumpSpeed;
    [HideInInspector] public bool canJump;
    public LayerMask groundMask;
    public Vector3 raycastOffset = new Vector3(0, 0.775f, 0);
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool isInCombo;
    [HideInInspector] public bool onGuard;
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
    readonly int m_HashOnGuard = Animator.StringToHash("OnGuard");

    [Space]
    [Header("Audio")]
    [SerializeField] AudioClip[] attackAudio;
    [SerializeField] AudioClip[] hurtAudio;
    [SerializeField] AudioClip[] deathAudio;
    int attackAudioLength, hurtAudioLength, deathAudioLength;
    AudioSource m_AudioSource;

    public PlayerInput m_PlayerInput;

    public CharacterController m_CharacterController;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    public float jumpButtonGracePeriod;
    public float lastGroundedGraceTime;
    Vector3 rootMotion;
    Vector3 velocity;
    bool m_IsGrounded;

    bool hasAttacked;
    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;
    const float k_AttackInputDuration = 0.03f;

    private void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);
    }

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
        isInCombo = false;
        GetOtherPlayer();
        SetPositions();
    }

    private void Start()
    {
        onGuard = false;
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
        CalculateIsGrounded();
        CalculateHorizontalMovement();
        CalculateVerticalMovement();

        m_animator.ResetTrigger(m_HashStateBasicAttack);
        
        if(hasAttacked)
        {
            m_animator.SetTrigger(m_HashStateBasicAttack);
            hasAttacked = false;
            canMove = false;
            isInCombo = true;
        }
        
        
        m_animator.SetBool(m_HashParameterIsInCombo, isInCombo);
        SetAnimations();
    }

    private void Update()
    {
        m_animator.SetFloat(m_HashStateTime, Mathf.Repeat(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        m_animator.SetBool(m_HashStateGrounded, m_IsGrounded);
    }

    void SetAttackTime()
    {
        if(m_AttackWaitCoroutine != null)
        {
            StopCoroutine(m_AttackWaitCoroutine);
        }
        m_AttackWaitCoroutine = StartCoroutine(AttackWait());
    }

    IEnumerator AttackWait()
    {
        hasAttacked = true;

        yield return m_AttackInputWait;

        hasAttacked = false;
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
        if(GameManager.sharedInstance.CurrentGameState == GameState.inGame && m_IsGrounded)
        {
            SetAttackTime();

            //PlayRandomAttackAudio();
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
        jumpButtonPressedTime = Time.time;
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

    void OnGuard(InputValue value)
    {
        float floatValue = value.Get<float>();
        print(floatValue);
        onGuard = floatValue == 1.0f ? true : false;
        m_animator.SetBool(m_HashOnGuard, onGuard);
    }

    void CalculateIsGrounded()
    {
        if(m_CharacterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }
        if(Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
        }
    }

    void CalculateHorizontalMovement()
    {
        m_HorizontalSpeed = axis.x;

        m_animator.SetFloat(m_HashStateHorizontalAxis, m_HorizontalSpeed, smoothInputSpeed, Time.fixedDeltaTime);
        m_animator.SetFloat(m_HashStateHorizontalAxisAbs, Mathf.Abs(m_HorizontalSpeed), smoothInputSpeed, Time.deltaTime);
    }

    void CalculateVerticalMovement()
    {
        if (m_IsGrounded)
        {
            canJump = true;
            m_VerticalSpeed = -0.5f;
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && canJump)
            {
                m_VerticalSpeed = jumpSpeed;
                canJump = false;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            m_VerticalSpeed += Physics.gravity.y * Time.fixedDeltaTime;
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 movement;
        if (m_IsGrounded)
        {
            var ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
            {
                 movement = Vector3.ProjectOnPlane(m_animator.deltaPosition, hitInfo.normal);
            }
            else
            {
                movement = m_animator.deltaPosition;
            }
        }
        else
        {
            movement = m_HorizontalSpeed * Vector3.right * Time.fixedDeltaTime * 3.5f;
        }
        movement += m_VerticalSpeed * Vector3.up * Time.fixedDeltaTime;
        movement.z = 0f;
        m_CharacterController.Move(movement);
        if (!m_IsGrounded)
        {
            m_animator.SetFloat(m_HashParameterVerticalVelocity, m_VerticalSpeed);
        }

        m_animator.SetBool(m_HashStateGrounded, m_IsGrounded);
    }

    private void SetAnimations()
    {
        if (m_HorizontalSpeed == 0)
        {
            m_animator.SetBool(m_HashStateIsWalking, false);
        }
        else
        {
            m_animator.SetBool(m_HashStateIsWalking, true);
        }
        if (otherPlayer.position.x > this.transform.position.x && !facingRight && m_IsGrounded)
        {
            facingRight = true;
            SetFacing();
        }
        else if (otherPlayer.position.x < this.transform.position.x && facingRight && m_IsGrounded)
        {
            facingRight = false;
            SetFacing();
        }
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
