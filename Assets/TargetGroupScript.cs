using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class TargetGroupScript : MonoBehaviour
{
    Transform target1;
    Transform target2;
    Cinemachine.CinemachineTargetGroup targetGroup;
    private void Awake()
    {
        targetGroup = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        target1 = GameObject.FindGameObjectWithTag("Player1").transform;
        target2 = GameObject.FindGameObjectWithTag("Player2").transform;

        targetGroup.AddMember(target1, 1f, 0f);
        targetGroup.AddMember(target2, 1f, 0f);
    }


}
