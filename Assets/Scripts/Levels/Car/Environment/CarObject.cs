using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    [SerializeField, Range(0f, 1f)]
    private float m_MaxPowerPerSec = 0.1f;          // 对应不同引擎每秒最大变化率
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
    private bool m_IsBrake;

    private void setupAttribute()
    {
        gameObject.layer = LayerMask.NameToLayer("Car");
    }

    private void setupRigibody()
    {
        m_Rigibody = GetComponent<Rigidbody>();
        m_Rigibody.isKinematic = false;
    }
    void Awake()
    {
        setupAttribute();
        setupRigibody();
    }

    public float getVelocity()
    {
        return m_Rigibody.velocity.magnitude;
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

    // 速度提高 -> 转向能力下降
    float computeTrueSteerAngle()
    {
        var percent = 1f - (m_Rigibody.velocity.magnitude / m_MaxSpeed);
        var steer = Mathf.Lerp(1f, m_WheelMaxRotation, percent);
        return steer;
    }

    // X轴为左右转向的目标，根据现有目标插值 (-1 ~ 1)
    // Z轴为前进的目标，依照发动机功率和响应延迟设定 ( -1 ~ 1)
    // Y轴正向为是否刹车
    public void updateUserInput(Vector3 input)
    {
        m_TargetSteerAngle = input.x * computeTrueSteerAngle();
        m_TargetEnginePercent = Mathf.Clamp(input.z, -1f, 1f);
        m_IsBrake = (input.y == 1f);
    }

    // 根据Target进行Current的移动
    void updataCurrentState()
    {
        if (m_TargetSteerAngle > m_CurrentSteerAngle)
        {
            m_CurrentSteerAngle += (m_MaxSteerPerSec * Time.deltaTime);
        }
        else
        {
            m_CurrentSteerAngle -= (m_MaxSteerPerSec * Time.deltaTime);
        }
        var abs = Mathf.Abs(m_TargetSteerAngle);
        m_CurrentSteerAngle = Mathf.Clamp(m_CurrentSteerAngle, -abs, abs);

        // 计算引擎现在功率百分比
        if (m_TargetEnginePercent > m_CurrentEnginePercent)
        {
            m_CurrentEnginePercent += (m_MaxPowerPerSec * Time.deltaTime);
        }
        else
        {
            m_CurrentEnginePercent -= Time.deltaTime / 100f;
            if (m_TargetEnginePercent < 0f)
            {
                m_CurrentEnginePercent -= (Time.deltaTime * m_CurrentEnginePercent / 2f);
            }
        }

        if (m_IsBrake)
        {
            m_CurrentEnginePercent -= Time.deltaTime / 2f;
        }

        m_CurrentEnginePercent = Mathf.Clamp(m_CurrentEnginePercent, 0f, 1f);
    }

    void updateRigidbody()
    {
        var velocity = computeCurrentSpeed();

        float fwd = computeForward(velocity);
        float shift = computeShift(velocity);
        float deltaAngle = computeObjectRotateAngle(shift) * Time.deltaTime;

        Vector3 deltaPosition = Vector3.forward * fwd + Vector3.right * shift;
        // 将速度映射到刚体上
        if (m_IsBrake)
        {
            var slowPercent = 0.6f;
            var slowVector = m_Rigibody.velocity - (m_Rigibody.velocity * slowPercent * Time.deltaTime);
            m_Rigibody.velocity = slowVector;
        }
        else if (deltaPosition.magnitude > m_Rigibody.velocity.magnitude)
        {
            m_Rigibody.velocity = transform.TransformDirection(deltaPosition);
        }
        else
        {
            var slowPercent = 0.001f;
            var slowVector = m_Rigibody.velocity - (m_Rigibody.velocity * slowPercent * Time.deltaTime);
            m_Rigibody.velocity = slowVector;
        }

        var currentRotator = transform.rotation;
        var deltaRototor = Quaternion.Euler(0f, deltaAngle, 0f);
        m_Rigibody.MoveRotation(currentRotator * deltaRototor);
    }

    void Update()
    {
        updataCurrentState();
        updateRigidbody();
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