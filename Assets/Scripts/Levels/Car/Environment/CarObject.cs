using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CarLevel
{
    [RequireComponent(typeof(Rigidbody))]
    // 控制车辆的接口, 实现移动的功能
    public class CarObject : MonoBehaviour, IPawnController
    {
        [SerializeField]
        private float m_WheelToCenter;
        [SerializeField, Range(1f, 100f)]
        private float m_MaxSpeed = 10f;
        [SerializeField, Range(0f, 10f)]
        private float m_ForwardSensitivity = 5f;
        [SerializeField, Range(0f, 10f)]
        private float m_SteerSensitivity = 5f;
        [SerializeField, Range(0f, 1f)]
        private float m_PowerSensitivity = 1f;

        [SerializeField]
        private AnimationCurve m_SpeedCurve;
        private Rigidbody m_Rigibody;

        /* Control */
        // RANGE -90 ~ 90
        private float m_TargetSteerAngle;
        private float m_CurrentSteerAngle;
        private float m_CurrentSteerRadius
        {
            get
            {
                return m_CurrentSteerAngle * Mathf.Deg2Rad;
            }
        }
        // RANGE 0 ~ 1
        private float m_CurrentEnginePercent;
        private float m_TargetEnginePercent;
        private float m_RefDeltaSpeed = 0f;
        private float m_Velocity;

        private void setupRigibody()
        {
            m_Rigibody = GetComponent<Rigidbody>();
            m_Rigibody.isKinematic = false;
        }
        void Start()
        {
            setupRigibody();
        }

        public float getVelocity(){
            return m_Velocity;
        }

        public Vector3 getWorldPosition(){
            return transform.position;
        }

        float computeDeltaShift(float deltaV)
        {
            float shift = Mathf.Sin(m_CurrentSteerRadius) * deltaV;
            return shift;
        }

        float computeDeltaForward(float deltaV)
        {
            float forward = Mathf.Cos(m_CurrentSteerRadius) * deltaV;
            return forward;
        }

        float computeObjectRotateAngle(float deltaShift)
        {
            float radius = Mathf.Atan2(deltaShift, m_WheelToCenter);
            return radius * Mathf.Rad2Deg;
        }

        float computeCurrentSpeed()
        {
            float current = m_SpeedCurve.Evaluate(m_CurrentEnginePercent) * m_MaxSpeed;
            return current;
        }

        public void updateUserInput(Vector3 input)
        {
            var deltaSteer = input.x * m_SteerSensitivity;
            var deltaForward = input.z * m_ForwardSensitivity;

            m_TargetEnginePercent += deltaForward * Time.deltaTime;
            m_TargetEnginePercent = Mathf.Clamp(m_TargetEnginePercent, 0f, 1f);

            m_TargetSteerAngle += deltaSteer * Time.deltaTime;
            m_TargetSteerAngle = Mathf.Clamp(m_TargetSteerAngle, -90f, 90f);
        }

        void updataCurrentState()
        {
            m_CurrentSteerAngle = m_TargetSteerAngle;
            m_CurrentEnginePercent = Mathf.SmoothDamp(m_CurrentEnginePercent, m_TargetEnginePercent, ref m_RefDeltaSpeed, m_PowerSensitivity);
        }

        void updateTransformation()
        {
            m_Velocity = computeCurrentSpeed() * Time.deltaTime;

            float deltaForward = computeDeltaForward(m_Velocity);
            float deltaShift = computeDeltaShift(m_Velocity);
            float rotateAngle = computeObjectRotateAngle(deltaShift);

            Vector3 currentRotator = transform.rotation.eulerAngles;
            Vector3 currentPosition = transform.position;

            Vector3 deltaPosition = Vector3.forward * deltaForward +
            Vector3.right * deltaShift;

            m_Rigibody.MovePosition(currentPosition + transform.TransformDirection(deltaPosition));

            currentRotator.y += rotateAngle;
            m_Rigibody.MoveRotation(Quaternion.Euler(currentRotator));
        }

        void Update()
        {
            updataCurrentState();
            updateTransformation();
        }

    }

}