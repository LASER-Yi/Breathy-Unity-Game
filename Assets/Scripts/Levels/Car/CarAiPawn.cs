using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 自动往道路前方移动
// 根据AI的不同特性，自动插队和自动补上前方空位

namespace CarLevel
{
    public class CarAiPawn : MonoBehaviour
    {
        private IPawnController m_Controller;

        [SerializeField]
        private float m_ReactionTime;
        [SerializeField]
        private float m_SafeDistance;
        [SerializeField]
        private float m_SteerSpeed;
        [SerializeField]
        private float m_MaxSpeed;
        void Awake()
        {
            m_Controller = GetComponent<IPawnController>();
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
