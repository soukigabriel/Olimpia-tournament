using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndJump : MonoBehaviour
{
    public PlayerController m_PlayerController;

    void JumpEnded()
    {
        m_PlayerController.canJump = true;
    }
}
