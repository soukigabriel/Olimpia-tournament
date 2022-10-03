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

    [Space]
    [Header("Animations variables")]
    Animator m_animator;
    const string STATE_WALKING = "isMoving";
    bool facingRight = true;


    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody>();
        SetPositions();
    }

    private void FixedUpdate()
    {
        Move(GetAxis());
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

    Vector3 GetAxis()
    {
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , 0);
        return movement;
    }

}
