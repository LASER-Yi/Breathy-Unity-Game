using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(Rigidbody))]

// 控制车辆的接口, 实现移动的功能
// TODO: 将车辆物理改成依靠受力
public class CarController : MonoBehaviour, IPawnController
{
    [SerializeField]
    private float m_WheelPosition;                  // 对应不同车辆的前后轮距离
    [SerializeField, Range(0f, 90f)]
    private float m_FrontWheelRotation = 30f;
    [SerializeField, Range(1f, 100f)]
    private float m_MaxSpeed = 10f;                 // 对应不同车辆的极速
    [SerializeField, Range(0f, 0.4f)]
    private float m_Acceleration = 0.1f;          // 对应加速度

    [SerializeField]
    private AnimationCurve m_SpeedCurve;            // 对应不同发动机功率能达到的极速
    private Rigidbody m_Rigibody;

    public float getWheelPosition()
    {
        return m_WheelPosition;
    }

    /* Control */
    // RANGE -90 ~ 90

    private float m_CurrentSteerAngle;

    private float m_DynamicFwRotation
    {
        get
        {
            var percent = 1f - (m_CurrentVelocity / m_MaxSpeed);
            return Mathf.Lerp(1f, m_FrontWheelRotation, percent);
        }
    }

    // 前轮转向弧度
    private float m_DynamicSteerRadius
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

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Car");
        m_RoadLayer = 1 << LayerMask.NameToLayer("Road");
        m_Rigibody = GetComponent<Rigidbody>();
        m_Rigibody.isKinematic = false;
    }

    public float getVelocity()
    {
        return m_CurrentVelocity;
    }

    public void setEnginePower(float percent){
        m_CurrentEnginePercent = percent;
    }

    public Vector3 getWorldPosition()
    {
        return transform.position;
    }

    private LayerMask m_RoadLayer;
    protected RoadChunk m_CurrentAttachRoad;

    protected void updateRoadState()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, m_RoadLayer))
        {
            var road = hit.transform.GetComponentInParent<RoadChunk>();
            if (road != null)
            {
                if (m_CurrentAttachRoad != null)
                {
                    if (m_CurrentAttachRoad != road)
                    {
                        // 更新路
                        m_CurrentAttachRoad.removeCarFromRoad();
                        road.addCarToRoad();
                        m_CurrentAttachRoad = road;
                        // m_ConservativeAi.updateRoadState(m_AttachRoad.getRoadNum());
                    }
                }
                else
                {
                    road.addCarToRoad();
                    m_CurrentAttachRoad = road;
                    // m_ConservativeAi.updateRoadState(m_AttachRoad.getRoadNum());
                }
            }
        }else{
            Destroy(gameObject);
        }
    }

    public bool isRoadInfoAvaliable()
    {
        if (m_CurrentAttachRoad == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public RoadChunk getRoadInfoChunk()
    {
        if (m_CurrentAttachRoad == null) Debug.LogAssertion("请检查路面信息!!!!");
        return m_CurrentAttachRoad;
    }

    float computeShift(float turnRadius, float deltaV)
    {
        float shift = Mathf.Sin(turnRadius) * deltaV;
        return shift;
    }

    float computeForward(float turnRadius, float deltaV)
    {
        float forward = Mathf.Cos(turnRadius) * deltaV;
        return forward;
    }

    float computeObjectRotateAngle(float deltaShift)
    {
        float radius = Mathf.Atan2(deltaShift, m_WheelPosition);
        return radius * Mathf.Rad2Deg;
    }

    // X轴为左右转向的目标，根据现有目标插值 (-1 ~ 1)
    // Z轴为前进的目标，依照发动机功率和响应延迟设定 ( -1 ~ 1)
    // Y轴正向为是否刹车
    public void updateUserInput(Vector3 input)
    {
        m_CurrentSteerAngle = input.x * m_DynamicFwRotation;
        m_TargetEnginePercent = Mathf.Clamp(input.z, -1f, 1f);
        m_IsBrake = (input.y == 1f);
    }

    float updateEngineOutput()
    {
        m_CurrentEnginePercent -= Time.deltaTime / 50f;

        if (m_IsBrake)
        {
            m_CurrentEnginePercent -= Time.deltaTime * m_CurrentEnginePercent;
        }
        else if (m_TargetEnginePercent > m_CurrentEnginePercent)
        {
            m_CurrentEnginePercent += m_Acceleration * Time.deltaTime;
        }
        else if (m_TargetEnginePercent < 0f)
        {
            m_CurrentEnginePercent += Time.deltaTime * m_TargetEnginePercent / 10f;
        }
        m_CurrentEnginePercent = Mathf.Clamp(m_CurrentEnginePercent, 0f, 1f);
        return m_CurrentEnginePercent;
    }

    private float m_CurrentVelocity = 0f;

    float computeCurrentSpeed(float powerPercent)
    {
        // 根据引擎功率计算现在的速度
        float current = m_SpeedCurve.Evaluate(powerPercent) * m_MaxSpeed;
        m_CurrentVelocity = current;
        return current;
    }

    void updateRigidbody(float powerPercent, float turnRadius)
    {
        var velocity = computeCurrentSpeed(powerPercent);

        float fwd = computeForward(turnRadius, velocity);
        float shift = computeShift(turnRadius, velocity);

        Vector3 deltaPosition = Vector3.forward * fwd + Vector3.right * shift;

        m_Rigibody.MovePosition(transform.position + transform.TransformDirection(deltaPosition * Time.deltaTime));

        float deltaAngle = computeObjectRotateAngle(shift) * Time.deltaTime;
        var currentRotator = transform.rotation;
        var deltaRototor = Quaternion.Euler(0f, deltaAngle, 0f);
        if (deltaRototor != Quaternion.identity)
        {
            m_Rigibody.MoveRotation(currentRotator * deltaRototor);
        }
    }

    void Update()
    {
        updateRoadState();

        var power = updateEngineOutput();
        var turn = m_DynamicSteerRadius;
        updateRigidbody(power, turn);
    }

    void OnDrawGizmos()
    {
        var origin = transform.position;
        var power = m_CurrentEnginePercent;
        var turn = m_CurrentSteerAngle;

        // 可视化车辆控制信息
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.TransformDirection(Vector3.forward) * power);
        Gizmos.DrawLine(origin, origin + transform.TransformDirection(Vector3.right) * (turn / m_FrontWheelRotation));
    }

}

class CarControllerEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmosSelected(CarController script, GizmoType type)
    {
        // Draw wheel position

        var center = script.transform.position;
        var length = script.getWheelPosition();
        var width = 1f;

        var frontLeft = center + (Vector3.left * width + Vector3.forward * length) / 2f;
        var frontRight = center + (Vector3.right * width + Vector3.forward * length) / 2f;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(frontLeft, frontRight);

        var backLeft = center + (Vector3.left * width + Vector3.back * length) / 2f;
        var backRight = center + (Vector3.right * width + Vector3.back * length) / 2f;
        Gizmos.DrawLine(backLeft, backRight);
    }
}