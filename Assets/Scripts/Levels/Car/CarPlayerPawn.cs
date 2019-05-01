using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLevel
{
    // 车辆行为
    // 左右转只能在前进时候进行
    // 松开油门会减速

    [RequireComponent(typeof(CarObject))]
    public class CarPlayerPawn : MonoBehaviour
    {
        private IPawnController m_Controller;

        void Awake()
        {
            m_Controller = GetComponent<IPawnController>();
        }

        void Update()
        {
            var input = handleUserInput();
            if (m_Controller != null) m_Controller.updateUserInput(input);
        }

        public Vector3 handleUserInput()
        {
            Vector3 input = Vector3.zero;
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool handbrake = Input.GetButton("Jump");
            input += Vector3.up * (handbrake ? 1.0f : 0.0f);
            input += Vector3.forward * vertical;
            input += Vector3.right * horizon;
            return input;
        }
    }
}