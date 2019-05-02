using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CarAiAttribute;

public class ManualAiPawn : CarAiPawn
{
    [SerializeField, Range(-1f,1f)]
    private float m_ForwardSpeed = 1f;
    [SerializeField, Range(0, 2)]
    private int m_TargetRoadNumber = 1;
    [SerializeField]
    private bool m_IsBrake = false;
    void Update(){
        checkRoadState();
        if (m_AttachRoad != null)
        {
            var stra = Strategic.Default;
            stra.brake = m_IsBrake;
            stra.power = m_ForwardSpeed;
            stra.targetRoadNumber = m_TargetRoadNumber;
            var action = generateAction(stra);
            m_Controller.updateUserInput(action);
        }
    }
}
