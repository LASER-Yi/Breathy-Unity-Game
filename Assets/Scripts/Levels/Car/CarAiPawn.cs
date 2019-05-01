using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 自动往道路前方移动
// 根据AI的不同特性，自动插队和自动补上前方空位

namespace CarLevel
{
    [RequireComponent(typeof(CarObject))]
    public class CarAiPawn : MonoBehaviour
    {
        enum EAiType
        {
            Radical,        // 激进型Ai
            Conservative,   // 保守型Ai
        }
        private IPawnController m_Controller;

        [SerializeField]
        private float m_ReactionTime;
        [SerializeField]
        private float m_SteerSpeed;
        [SerializeField]
        private float m_MaxSpeed;

        private RoadObject m_AttachRoad;

        // Ai Parameters

        // 探测范围: danger -> closer -> safe
        [SerializeField]
        private float m_SafeDistance;
        [SerializeField]
        private float m_CloserDistance;

        void Awake()
        {
            m_Controller = GetComponent<IPawnController>();
        }



        void Update(){

        }
    }
}
