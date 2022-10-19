using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxes : MonoBehaviour
{
    [SerializeField] BoxCollider rightPunchBC;
    [SerializeField] BoxCollider kickBC;

    void EnableRightPunch()
    {
        rightPunchBC.enabled = true;
    }
    void DisableRightPunch()
    {
        rightPunchBC.enabled = false;
    }
    void EnableKickOne()
    {
        kickBC.enabled = true;
    }
    void DisableKickOne()
    {
        kickBC.enabled = false;
    }

}
