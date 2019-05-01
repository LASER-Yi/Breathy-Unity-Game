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

        [SerializeField]
        private float m_ReactionTime;
        private RoadObject m_AttachRoad;

        // Ai Parameters

        // 探测范围: danger -> closer -> safe
        [SerializeField]
        private float m_SafeDistance;
        [SerializeField]
        private float m_CloserDistance;

        /* State */
        private int m_CurrentRoadNumber
        {
            get
            {
                if (m_AttachRoad != null)
                {
                    return m_AttachRoad.computeRoadNumberWorld(m_CurrentHorizonal);
                }
                else
                {
                    return -1;
                }
            }
        }
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

        void updateStateFromRoad()
        {
            if (m_AttachRoad != null)
            {
                // m_CurrentRoadNumber = 
            }
        }

        Environment detectEnvironment()
        {
            return new Environment();
        }

        // 根据当前环境情况设置决策（加减速, 车道数）
        Strategic computeStrategic(Environment env)
        {
            var stra = new Strategic();
            stra.power = 1f;
            stra.targetRoadNumber = 0;
            stra.brake = false;
            return stra;
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
        float computeTurnPercent(float target, float current)
        {
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
            var current = m_AttachRoad.getHorizonalProject(selfDirection);
            return current;
        }

        // 根据决策函数的输入行动
        Vector3 generateAction(Strategic stra)
        {
            var action = Vector3.zero;
            action += Vector3.forward * stra.power;
            action += Vector3.up * (stra.brake ? 1.0f : 0.0f);

            var targetHorizonal = m_AttachRoad.computeRoadCenterWorld(stra.targetRoadNumber);
            var targetOffset = targetHorizonal - m_CurrentHorizonal;

            var target = computeTargetProjection(targetOffset);
            var current = computeCurrentTurnProject();   // -1 ~ 0 ~ 1

            action += Vector3.right * computeTurnPercent(target, current);

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

        void OnDrawGizmos()
        {
            var origin = transform.position;
            var target = origin + Vector3.right * m_DebugTurnPercent;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, target);
        }
    }
}
