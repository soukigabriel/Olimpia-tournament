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

    [Space]
    [Header("Movement variables")]
    public Vector3 movement;
    [SerializeField] float speed;
    [SerializeField] Rigidbody m_rigidBody;
    [SerializeField] float jumpForce;
    public bool canJump;
    public LayerMask groundMask;
    [SerializeField] float rayLenght = 2f;
    Vector3 raycastOffset = new Vector3(0, 0.775f, 0);
    public bool canMove;
    [SerializeField] bool isGrounded;
    public bool isInCombo;

    [Space]
    [Header("Animations variables")]
    [SerializeField] Animator m_animator;
    public bool facingRight = true;
    readonly int m_HashStateTime = Animator.StringToHash("StateTime");
    readonly int m_HashStateBasicAttack = Animator.StringToHash("BasicAttack");
    readonly int m_HashStateIsWalking = Animator.StringToHash("IsWalking");
    readonly int m_HashStateHorizontalVelocity = Animator.StringToHash("HorizontalVelocity");
    readonly int m_HashStatePositionDiference = Animator.StringToHash("PositionDiference");
    readonly int m_HashStateGrounded = Animator.StringToHash("Grounded");
    int m_HashParameterIsInCombo = Animator.StringToHash("IsInCombo");


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

    private void Awake()
    {
        isInCombo = false;
        GetOtherPlayer();
        m_rigidBody = GetComponent<Rigidbody>();
        SetPositions();
    }
    

    private void Start()
    {
        //Time.timeScale = 0.05f;
        canJump = true;
        canMove = true;
    }

    private void FixedUpdate()
    {
        Move(movement);
        m_animator.ResetTrigger(m_HashStateBasicAttack);
    }

    private void Update()
    {
        IsTouchingTheGround();
        m_animator.SetFloat(m_HashStateTime, Mathf.Repeat(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        //Jump();
        Debug.DrawLine(this.transform.position + raycastOffset, this.transform.localPosition + raycastOffset + (Vector3.down * rayLenght), Color.red);
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

        SetFacing();
    }

    void OnBasicAttack()
    {
        m_animator.SetTrigger(m_HashStateBasicAttack);
        canMove = false;
        isInCombo = true;
        m_animator.SetBool(m_HashParameterIsInCombo, isInCombo);
        //canMove = false;
    }

    void SetCanMove()
    {
        canMove = true;
    }

    void OnJump()
    {
        if(isGrounded && !isInCombo)
        {
            m_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //canJump = false;
        }
    }


    void IsTouchingTheGround()
    {
        if(Physics.Raycast(this.transform.position + raycastOffset, Vector2.down, rayLenght, groundMask))
        {
            isGrounded = true;
            m_animator.SetBool(m_HashStateGrounded, isGrounded);
        }
        else
        {
            isGrounded = false;
            m_animator.SetBool(m_HashStateGrounded, isGrounded);
        }
    }

    void OnMove(InputValue movementValue)
    {
        movement = movementValue.Get<Vector2>();
    }

    void Move(Vector3 movement)
    {
        if (canMove)
        {
            m_rigidBody.velocity = new Vector3(movement.x * speed * Time.fixedDeltaTime, m_rigidBody.velocity.y, m_rigidBody.velocity.z);
        }
        else if(!canMove && isGrounded)
        {
            m_rigidBody.velocity = new Vector3 (0f, m_rigidBody.velocity.y , 0f);
        }
        else if(!canMove && !isGrounded)
        {
            m_rigidBody.velocity = new Vector3 (m_rigidBody.velocity.x, m_rigidBody.velocity.y , 0f);
        }
            SetAnimations();
    }


    private void SetAnimations()
    {
        if (m_rigidBody.velocity.x == 0)
        {
            m_animator.SetBool(m_HashStateIsWalking, false);
        }
        else
        {
            m_animator.SetBool(m_HashStateIsWalking, true);
        }
        if (otherPlayer.position.x > this.transform.position.x && !facingRight && isGrounded)
        {
            facingRight = true;
            SetFacing();
        }
        else if (otherPlayer.position.x < this.transform.position.x && facingRight && isGrounded)
        {
            facingRight = false;
            SetFacing();
        }
        m_animator.SetFloat(m_HashStateHorizontalVelocity, m_rigidBody.velocity.x);
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
