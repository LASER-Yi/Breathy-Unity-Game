using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CarLevel
{
    // 车辆行为
    // 左右转只能在前进时候进行
    // 松开油门会减速
    public class CarPlayerPawn : MonoBehaviour, IGamePawnBase
    {
        [SerializeField]
        private AnimationCurve m_AccelerationCurve;
        [SerializeField, Range(1f, 100f)]
        private float m_MaxSpeed;

        [SerializeField]
        private AnimationCurve m_TurnSensitivityCurve;

        /* Componment */

        /* Control */
        private float m_TargetEnginePercent;
        private float m_CurrentEnginePercent;

        // Update is called once per frame
        void Update()
        {
            Vector3 input = handlePlayerInput();
            movePawnBy(input);
        }

        private Vector3 handlePlayerInput(){
            Vector3 input = Vector3.zero;
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            bool handbrake = Input.GetButton("Jump");
            input += (Vector3.forward * vertical * (handbrake?0.0f:1.0f));
            input += Vector3.right * horizon;
            return input;
        }

        public void movePawnBy(Vector3 localDirection){
            if(localDirection.z != 0f){
                m_CurrentEnginePercent += (Time.deltaTime * localDirection.z);
            }else{
                // 刹车
                m_CurrentEnginePercent -= Time.deltaTime;
            }

            m_CurrentEnginePercent = Mathf.Clamp(m_CurrentEnginePercent, 0f, 1f);

            float currentSpeed = computeEngineOutputPercent() * m_MaxSpeed;

            float deltaX = localDirection.x;
            deltaX *= computeTurnPercent(currentSpeed);
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation += Vector3.up * deltaX;

            Vector3 localDirTran = localDirection;
            localDirTran.z = currentSpeed * Time.deltaTime;
            localDirTran.x = 0f;

            Vector3 deltaWorldDir = transform.TransformDirection(localDirTran) + transform.position;
            transform.SetPositionAndRotation(deltaWorldDir, Quaternion.Euler(rotation));
        }

        private float computeTurnPercent(float currentSpeed){
            float precent = currentSpeed / m_MaxSpeed;
            float output = m_TurnSensitivityCurve.Evaluate(precent);
            return output;
        }

        private float computeEngineOutputPercent(){
            float _input = Mathf.Clamp(m_CurrentEnginePercent, 0f, 1f);
            float output = m_AccelerationCurve.Evaluate(_input);
            return output;
        }
    }
}