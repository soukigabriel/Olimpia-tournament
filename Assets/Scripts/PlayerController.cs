using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum Player { Player1, Player2 }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]


public class PlayerController : MonoBehaviour
{
    [SerializeField] Player thisPlayer;

    [Header("Movement variables")]
    Vector3 movement;
    [SerializeField] float speed;
    Rigidbody m_rigidBody;
    [SerializeField] float jumpForce;
    public bool canJump;
    public LayerMask groundMask;
    [SerializeField] float rayLenght = 2f;
    Vector3 raycastOffset = new Vector3(0, 0.775f, 0);

    [Space]
    [Header("Animations variables")]
    Animator m_animator;
    const string STATE_WALKING = "isMoving";
    const string STATE_JUMP = "isJumping";
    const string STATE_RIGHT_PUNCH = "rightPunch";
    const string STATE_LEFT_PUNCH = "leftPunch";
    bool facingRight = true;
    public bool canRightPunch, canLeftPunch;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody>();
        SetPositions();
    }

    private void Start()
    {
        canRightPunch = true;
        canLeftPunch = false;
        canJump = true;
    }

    private void FixedUpdate()
    {
        Move(GetAxis());
    }

    private void Update()
    {
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

    void Move(Vector3 movement)
    {
        m_rigidBody.velocity = new Vector3(movement.x * speed * Time.fixedDeltaTime, m_rigidBody.velocity.y, m_rigidBody.velocity.z);
        SetAnimations();
    }

    void Punch()
    {
        if (Input.GetButtonDown("Fire1") && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Right Punch"))
        {
            m_animator.SetTrigger(STATE_RIGHT_PUNCH);
            m_animator.ResetTrigger(STATE_LEFT_PUNCH);
        }
        else if (Input.GetButtonDown("Fire1") && !m_animator.GetCurrentAnimatorStateInfo(0).IsName("Left Punch"))
        {
            m_animator.SetTrigger(STATE_LEFT_PUNCH);
        }

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
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 GetAxis()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , 0);
        return movement;
    }



    private void SetAnimations()
    {
        if (m_rigidBody.velocity.x == 0)
        {
            m_animator.SetBool(STATE_WALKING, false);
        }
        else
        {
            m_animator.SetBool(STATE_WALKING, true);
        }
        if (m_rigidBody.velocity.x < 0)
        {
            facingRight = false;
        }
        else if(m_rigidBody.velocity.x > 0)
        {
            facingRight = true;
        }
        SetFacing();
    }

    void SetFacing()
    {
        if (facingRight)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(1, 1, -1);
        }
    }


}
