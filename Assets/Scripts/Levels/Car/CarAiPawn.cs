using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 自动往道路前方移动
// 根据AI的不同特性，自动插队和自动补上前方空位

[RequireComponent(typeof(CarObject))]
public class CarAiPawn : MonoBehaviour
{
    enum EAiType
    {
        Radical,        // 激进型Ai
        Conservative,   // 保守型Ai
    }

    [SerializeField]
    private EAiType m_AiType = EAiType.Conservative;

    struct Environment
    {
        public float? frontDistance;        // 前方物体的距离
        public float? leftClosetDistance;   // 左侧物体最近距离 前(+) -> 0 -> 后(-)
        public float? rightClosetDistance;  // 右侧物体最近距离 前(+) -> 0 -> 后(-)
        public int currentRoadNumber;      // 当前
    }

    struct Strategic
    {
        public float power;        // 0 ~ 1
        public bool brake;         // override power
        public int targetRoadNumber;
    }
    private IPawnController m_Controller;

    private LayerMask m_RoadLayer;
    private LayerMask m_ObstructLayer;

    [SerializeField]
    private float m_ReactionTime;
    private RoadObject m_AttachRoad;

    // Ai Parameters

    // 探测范围: danger -> closer -> safe
    [SerializeField]
    private float m_SafeDistance;
    [SerializeField]
    private float m_CloserDistance;

    public float getSafeDistance()
    {
        return m_SafeDistance;
    }
    public float getCloserDistance()
    {
        return m_CloserDistance;
    }

    /* State */
    private float m_CurrentHorizonal
    {
        get
        {
            return transform.position.x;
        }
    }


    void Awake()
    {
        m_Controller = GetComponent<IPawnController>();
        m_RoadLayer = 1 << LayerMask.NameToLayer("Road");
        m_ObstructLayer = 1 << LayerMask.NameToLayer("Car");
    }

    void checkRoadState()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, m_RoadLayer))
        {
            var road = hit.transform.GetComponentInParent<RoadObject>();
            if (road != null)
            {
                if (m_AttachRoad != null)
                {
                    if (m_AttachRoad != road)
                    {
                        m_AttachRoad.removeAiFromRoad(this);
                        road.addAiToRoad(this);
                        m_AttachRoad = road;
                    }
                }
                else
                {
                    road.addAiToRoad(this);
                    m_AttachRoad = road;
                }
            }
        }
    }

    private Collider[] collResults = new Collider[20];
    private int collCount = 0;

    bool isDetectFront(Vector3 direction)
    {
        var fwd = transform.TransformDirection(Vector3.forward);
        return Vector3.Dot(fwd, direction) > 0 ? true : false;
    }

    Environment detectEnvironment()
    {
        var env = new Environment();
        env.currentRoadNumber = m_AttachRoad.computeRoadNumberWorld(m_CurrentHorizonal);

        var center = transform.position;
        var frontDetect = center + Vector3.forward * m_SafeDistance;
        var backDetect = center + Vector3.back * m_SafeDistance;
        var radius = m_AttachRoad.getRoadWidth() * 3f;
        collCount = Physics.OverlapCapsuleNonAlloc(frontDetect, backDetect, radius, collResults, m_ObstructLayer);
        for (int i = 0; i < collCount; ++i)
        {
            var col = collResults[i];
            if (col.transform == transform) continue;

            var direction = col.transform.position - transform.position;
            var isFront = isDetectFront(direction);
            var distance = Vector3.Distance(transform.position, col.transform.position);
            var projection = m_AttachRoad.getHorizonalProject(direction);

            if (Mathf.Abs(projection) < 0.5f)
            {
                // Current
                if (isFront)
                {
                    if (env.frontDistance.HasValue)
                    {
                        if (env.frontDistance.Value < distance)
                        {
                            env.frontDistance = distance;
                        }
                    }
                    else
                    {
                        env.frontDistance = distance;
                    }
                }
            }
            else if (Mathf.Abs(projection) < 1.5f)
            {
                if (projection < 0)
                {
                    // Left
                }
                else
                {
                    // Right
                }
            }
        }
        return env;
    }

    Strategic computeConservativeAi(in Environment env)
    {
        var stra = new Strategic();
        stra.brake = false;
        stra.power = 1f;
        stra.targetRoadNumber = 1;
        return stra;
    }

    Strategic computeRaidcalAi(in Environment env)
    {
        var stra = new Strategic();
        stra.brake = false;
        stra.power = 1f;
        stra.targetRoadNumber = 1;
        return stra;
    }

    // 根据当前环境情况设置决策（加减速, 车道数）
    Strategic computeStrategic(Environment env)
    {
        switch (m_AiType)
        {
            case EAiType.Conservative:
                return computeConservativeAi(in env);
            case EAiType.Radical:
                return computeRaidcalAi(in env);
            default:
                var stra = new Strategic();
                stra.power = 1f;
                stra.targetRoadNumber = 1;
                stra.brake = false;
                return stra;
        }
    }

    // 根据决策函数的输入行动
    Vector3 generateAction(Strategic stra)
    {
        var action = Vector3.zero;
        action += Vector3.forward * stra.power;
        action += Vector3.up * (stra.brake ? 1.0f : 0.0f);

        var targetHorizonal = m_AttachRoad.computeRoadCenterWorld(stra.targetRoadNumber);
        var targetOffset = targetHorizonal - m_CurrentHorizonal;

        action += Vector3.right * computeTurnPercent(targetOffset);

        return action;
    }

    void Update()
    {
        // Note: 为了避免影响帧数，请每帧不要进行太多探测
        checkRoadState();
        if (m_AttachRoad != null)
        {
            var env = detectEnvironment();
            var stra = computeStrategic(env);
            var action = generateAction(stra);
            m_Controller.updateUserInput(action);
        }
    }

    // 根据当前所在位置和目标位置的百分比
    // 生成此时的目标转向在水平轴上的投影数值
    // -1
    float computeTargetProjection(float offset)
    {
        var width = m_AttachRoad.getRoadWidth();
        var percent = offset / width; // -1 ～ 0 ～ 1
        percent = Mathf.Clamp(percent, -1f, 1f);
        return percent;
    }

    private float m_DebugTurnPercent = 0f;

    // 传入在水平轴上的投影，得到目标转向角度
    float computeTurnPercent(float offset)
    {
        var target = computeTargetProjection(offset);
        var current = computeCurrentTurnProject();   // -1 ~ 0 ~ 1

        var turnPercent = target - current;
        // 加快回正速度
        if (turnPercent * current < 0f)
        {
            turnPercent *= 5f;
        }
        else
        {
            turnPercent /= 5f;
        }
        turnPercent = Mathf.Clamp(turnPercent, -1f, 1f);

        m_DebugTurnPercent = turnPercent;

        return turnPercent;
    }

    float computeCurrentTurnProject()
    {
        var selfDirection = transform.TransformDirection(Vector3.forward);
        var current = m_AttachRoad.getDegreeProjection(selfDirection);
        return current;
    }

    void OnDrawGizmos()
    {
        var origin = transform.position;
        var target = origin + Vector3.right * m_DebugTurnPercent;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, target);

        Gizmos.color = Color.green;
        for (int i = 0; i < collCount; ++i)
        {
            var position = collResults[i].transform.position;
            Gizmos.DrawLine(origin, position);
        }
    }
}

[CustomEditor(typeof(CarAiPawn))]
public class CarAiPawnEditor : Editor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
    static void DrawGizmosSelected(CarAiPawn script, GizmoType type)
    {

        var center = script.transform.position;
        // Draw Detect Circle
        var safe = script.getSafeDistance();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, safe);

        var closer = script.getCloserDistance();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, closer);
    }
}
