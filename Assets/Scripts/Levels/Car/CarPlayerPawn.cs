using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLevel
{
    // 车辆行为
    // 左右转只能在前进时候进行
    // 松开油门会减速


    public class CarPlayerPawn : MonoBehaviour
    {
        private IGamePawnBaseController m_Controller;

        void Awake()
        {
            m_Controller = GetComponent<IGamePawnBaseController>();
        }

        void Update()
        {
            var input = handleUserInput();
            if(m_Controller != null) m_Controller.updateUserInput(input);
        }

        public Vector3 handleUserInput(){
            Vector3 input = Vector3.zero;
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool handbrake = Input.GetButton("Jump");
            input += (Vector3.forward * vertical * (handbrake ? 0.0f : 1.0f));
            input += Vector3.right * horizon;
            return input;
        }
    }
}