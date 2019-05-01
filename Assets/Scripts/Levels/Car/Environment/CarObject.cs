using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CarLevel
{
    [RequireComponent(typeof(Rigidbody))]

    // 控制车辆的接口, 实现移动的功能
    // TODO: 将车辆物理改成依靠受力
    public class CarObject : MonoBehaviour, IPawnController
    {
        [SerializeField]
        private float m_WheelToCenter;                  // 对应不同车辆的前后轮距离
        [SerializeField, Range(0f, 90f)]
        private float m_WheelMaxRotation = 60f;
        [SerializeField, Range(1f, 100f)]
        private float m_MaxSpeed = 10f;                 // 对应不同车辆的极速
        [SerializeField, Range(0f, 10f)]
        private float m_MaxPowerPerSec = 5f;          // 对应不同引擎每秒最大变化率
        [SerializeField, Range(0f, 30f)]
        private float m_MaxSteerPerSec = 10f;            // 对应方向盘重量，每秒最大转向角度

        [SerializeField]
        private AnimationCurve m_SpeedCurve;            // 对应不同发动机功率能达到的极速
        private Rigidbody m_Rigibody;

        /* Control */
        // RANGE -90 ~ 90
        private float m_TargetSteerAngle;
        private float m_CurrentSteerAngle;

        // 前轮转向弧度
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
        private bool m_IsBrake;

        private void setupRigibody()
        {
            m_Rigibody = GetComponent<Rigidbody>();
            m_Rigibody.isKinematic = false;
        }
        void Start()
        {
            setupRigibody();
        }

        public float getVelocity()
        {
            return m_Velocity;
        }

        public Vector3 getWorldPosition()
        {
            return transform.position;
        }

        float computeShift(float deltaV)
        {
            float shift = Mathf.Sin(m_CurrentSteerRadius) * deltaV;
            return shift;
        }

        float computeForward(float deltaV)
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
            // 根据引擎功率计算现在的速度
            float current = m_SpeedCurve.Evaluate(m_CurrentEnginePercent) * m_MaxSpeed;
            return current;
        }

        // X轴为左右转向的目标，根据现有目标插值 (-1 ~ 1)
        // Z轴为前进的目标，依照发动机功率和响应延迟设定 ( -1 ~ 1)
        // Y轴正向为是否刹车
        public void updateUserInput(Vector3 input)
        {
            m_TargetSteerAngle = input.x * m_WheelMaxRotation;
            m_TargetEnginePercent = Mathf.Clamp(input.z, 0f, 1f);
            m_IsBrake = (input.y > 0.5);
        }

        // 根据Target进行Current的移动
        void updataCurrentState()
        {
            var steerDist = m_TargetSteerAngle - m_CurrentSteerAngle;
            if (steerDist > 0)
            {
                m_CurrentSteerAngle += (m_MaxSteerPerSec * Time.deltaTime);
            }
            else if (steerDist < 0)
            {
                m_CurrentSteerAngle -= (m_MaxSteerPerSec * Time.deltaTime);
            }
            var abs = Mathf.Abs(m_TargetSteerAngle);
            m_CurrentSteerAngle = Mathf.Clamp(m_CurrentSteerAngle, -abs, abs);

            // 计算引擎现在功率百分比
            var powerDist = m_TargetEnginePercent - m_CurrentEnginePercent;
            if (powerDist > 0)
            {
                m_CurrentEnginePercent += m_MaxPowerPerSec * Time.deltaTime;
            }
            else
            {
                m_CurrentEnginePercent -= m_MaxPowerPerSec * Time.deltaTime;
            }
            m_CurrentEnginePercent = Mathf.Clamp(m_CurrentEnginePercent, 0f, m_TargetEnginePercent);
        }

        void updatePosition()
        {
            m_Velocity = computeCurrentSpeed();

            float fwd = computeForward(m_Velocity);
            float shift = computeShift(m_Velocity);
            float deltaAngle = computeObjectRotateAngle(shift) * Time.deltaTime;

            Vector3 currentRotator = transform.rotation.eulerAngles;
            Vector3 currentPosition = transform.position;

            Vector3 deltaPosition = Vector3.forward * fwd + Vector3.right * shift;
            if (deltaPosition.magnitude > m_Rigibody.velocity.magnitude)
            {
                m_Rigibody.velocity = transform.TransformDirection(deltaPosition);
            }

            currentRotator.y += deltaAngle;
            m_Rigibody.MoveRotation(Quaternion.Euler(currentRotator));
        }

        void Update()
        {
            updataCurrentState();
            updatePosition();
        }

        void OnDrawGizmos()
        {
            var origin = transform.position;
            var power = m_CurrentEnginePercent;
            var turn = m_CurrentSteerAngle;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + transform.TransformDirection(Vector3.forward) * power);
            Gizmos.DrawLine(origin, origin + transform.TransformDirection(Vector3.right) * (turn / m_MaxSteerPerSec));
        }

    }

}