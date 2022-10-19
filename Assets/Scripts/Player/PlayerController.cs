using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum Player { Player1, Player2 }

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : MonoBehaviour
{
    public Quaternion rightRotation;
    public Quaternion leftRotation;

    [SerializeField] Player thisPlayer;

    Transform otherPlayer;
    [SerializeField]Transform m_player;

    [Header("Movement variables")]
    Vector3 movement;
    [SerializeField] float speed;
    Rigidbody m_rigidBody;
    [SerializeField] float jumpForce;
    public bool canJump;
    public LayerMask groundMask;
    [SerializeField] float rayLenght = 2f;
    Vector3 raycastOffset = new Vector3(0, 0.775f, 0);
    public bool canMove;

    [Space]
    [Header("Animations variables")]
    [SerializeField] Animator m_animator;
    const string STATE_JUMP = "isJumping";
    public bool facingRight = true;
    readonly int m_HashStateTime = Animator.StringToHash("StateTime");
    readonly int m_HashStateBasicAttack = Animator.StringToHash("BasicAttack");
    readonly int m_HashStateIsWalking = Animator.StringToHash("IsWalking");
    readonly int m_HashStateHorizontalVelocity = Animator.StringToHash("HorizontalVelocity");
    readonly int m_HashStatePositionDiference = Animator.StringToHash("PositionDiference");
    readonly int m_HashStateGrounded = Animator.StringToHash("Grounded");


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
        GetOtherPlayer();
        m_rigidBody = GetComponent<Rigidbody>();
        SetPositions();
    }

    private void Start()
    {
        canJump = true;
        canMove = true;
    }

    private void FixedUpdate()
    {
        Move(GetAxis());

    }

    private void Update()
    {

        m_animator.SetFloat(m_HashStateTime, Mathf.Repeat(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        Jump();
        Punch();
        //Debug.DrawLine(this.transform.position + raycastOffset, this.transform.localPosition + raycastOffset + (Vector3.down * rayLenght), Color.red);
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


    void Punch()
    {
        m_animator.ResetTrigger(m_HashStateBasicAttack);
        if (Input.GetButtonDown("Fire1"))
        {
            //canMove = false;
            m_animator.SetTrigger(m_HashStateBasicAttack);
        }
    }

    void SetCanMove()
    {
        canMove = true;
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && IsTouchingTheGround() && canJump)
        {
            m_rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            m_animator.SetTrigger(STATE_JUMP);
            canJump = false;
        }
    }

    public void EndJump()
    {
        canJump = true;
    }


    bool IsTouchingTheGround()
    {
        if(Physics.Raycast(this.transform.position + raycastOffset, Vector2.down, rayLenght, groundMask))
        {
            m_animator.SetBool(m_HashStateGrounded, true);
            return true;
        }
        else
        {
            m_animator.SetBool(m_HashStateGrounded, false);
            return false;
        }
    }

    Vector3 GetAxis()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , 0);
        return movement;
    }

    void Move(Vector3 movement)
    {
        if (canMove)
        {
            m_rigidBody.velocity = new Vector3(movement.x * speed * Time.fixedDeltaTime, m_rigidBody.velocity.y, m_rigidBody.velocity.z);
            SetAnimations();
        }
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
        if (otherPlayer.position.x > this.transform.position.x && !facingRight && IsTouchingTheGround())
        {
            facingRight = true;
            SetFacing();
        }
        else if (otherPlayer.position.x < this.transform.position.x && facingRight && IsTouchingTheGround())
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
