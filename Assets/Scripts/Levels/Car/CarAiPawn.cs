﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CarAiAttribute;

// 自动往道路前方移动
// 根据AI的不同特性，自动插队和自动补上前方空位

namespace CarAiAttribute
{
    public struct Strategic
    {
        public float power;        // 0 ~ 1
        public bool brake;         // override power
        public int targetRoadNumber;

        public static Strategic Default
        {
            get
            {
                var stra = new Strategic();
                stra.power = 1f;
                stra.brake = false;
                stra.targetRoadNumber = 1;
                return stra;
            }
        }
    }

    public struct Environment
    {
        public float? frontDistance;        // 前方物体的距离
        public float? leftDistance;         // 左侧物体最近距离
        public float? rightDistance;        // 右侧物体最近距离
        public float? backDistance;         // 后侧物体最近距离
        public float speed;
        public int roadNumber;
        public float timestamp;
    }
}

[RequireComponent(typeof(CarObject)), RequireComponent(typeof(BoxCollider))]
public class CarAiPawn : MonoBehaviour
{
    enum EAiType
    {
        Radical,        // 激进型Ai
        Conservative,   // 保守型Ai
    }

    [SerializeField]
    private EAiType m_AiType = EAiType.Conservative;

    private CarConservativeAi m_ConservativeAi = new CarConservativeAi();

    protected IPawnController m_Controller;
    private BoxCollider m_Collider;

    private LayerMask m_RoadLayer;
    private LayerMask m_ObstructLayer;

    [SerializeField]
    private float m_ReactionTime;
    protected RoadObject m_AttachRoad;

    // Ai Parameters

    // 探测范围: danger -> closer -> safe
    [SerializeField]
    private float m_SafeDistance;
    [SerializeField]
    private float m_WarnDistance;

    private float m_DynamicSafeDistance
    {
        get
        {
            var percent = Mathf.Clamp01(m_Controller.getVelocity() / 10f);
            return Mathf.SmoothStep(m_SafeDistance, 1.5f * m_SafeDistance, percent);
        }
    }

    public float getSafeDistance()
    {
        return m_SafeDistance;
    }
    public float getWarnDistance()
    {
        return m_WarnDistance;
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
        m_Collider = GetComponent<BoxCollider>();
        m_RoadLayer = 1 << LayerMask.NameToLayer("Road");
        m_ObstructLayer = 1 << LayerMask.NameToLayer("Car");
    }

    protected void checkRoadState()
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
                        // 更新路
                        m_AttachRoad.removeAiFromRoad(this);
                        road.addAiToRoad(this);
                        m_AttachRoad = road;
                        m_ConservativeAi.updateRoadState(m_AttachRoad.getRoadNum());
                    }
                }
                else
                {
                    road.addAiToRoad(this);
                    m_AttachRoad = road;
                    m_ConservativeAi.updateRoadState(m_AttachRoad.getRoadNum());
                }
            }
        }
    }

    private Collider[] collResults = new Collider[20];
    private int collCount = 0;

    private void updateColliders()
    {
        var center = transform.position;
        var halfExt = Vector3.zero;
        halfExt.y = 0.1f;
        halfExt.x = m_AttachRoad.getRoadWidth() * 1.5f;
        halfExt.z = m_DynamicSafeDistance;
        collCount = Physics.OverlapBoxNonAlloc(center, halfExt, collResults, Quaternion.identity, m_ObstructLayer);
    }

    bool isDetectFront(Vector3 direction)
    {
        var fwd = transform.TransformDirection(Vector3.forward);
        return Vector3.Dot(fwd, direction) > 0 ? true : false;
    }

    Environment computeEnvironment()
    {
        var env = new Environment();
        env.roadNumber = m_AttachRoad.computeRoadNumberWorld(m_CurrentHorizonal);
        env.speed = m_Controller.getVelocity();
        env.timestamp = Time.time;

        updateColliders();
        for (int i = 0; i < collCount; ++i)
        {
            var col = collResults[i];
            if (col.transform == transform) continue;

            var direction = col.transform.position - transform.position;
            var projection = m_AttachRoad.getHorizonalProject(direction);
            var distance = Vector3.Distance(transform.position, col.transform.position);

            var ray = new Ray(transform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, distance, m_ObstructLayer))
            {
                distance = hit.distance;
            }


            if (Mathf.Abs(projection) < 0.5f)
            {
                // Current
                if (isDetectFront(direction))
                {
                    if (env.frontDistance.HasValue)
                    {
                        env.frontDistance = Mathf.Min(distance, env.frontDistance.Value);
                    }
                    else
                    {
                        env.frontDistance = distance;
                    }
                }
                else
                {
                    if (env.backDistance.HasValue)
                    {
                        env.backDistance = Mathf.Min(distance, env.backDistance.Value);
                    }
                    else
                    {
                        env.backDistance = distance;
                    }
                }
            }
            else if (Mathf.Abs(projection) < 1.5f)
            {
                if (projection < 0)
                {
                    // Left
                    if (env.leftDistance.HasValue)
                    {
                        env.leftDistance = Mathf.Min(distance, env.leftDistance.Value);
                    }
                    else
                    {
                        env.leftDistance = distance;
                    }
                }
                else
                {
                    // Right
                    var assignment = distance;
                    if (env.rightDistance.HasValue)
                    {
                        env.rightDistance = Mathf.Min(distance, env.rightDistance.Value);
                    }
                    else
                    {
                        env.rightDistance = distance;
                    }
                }
            }
        }
        return env;
    }

    // 激进型Ai
    Strategic computeRaidcalAi(in Strategic prev, in Environment env)
    {
        var stra = Strategic.Default;
        return stra;
    }

    Strategic computeStrategic(Environment env)
    {
        var stra = Strategic.Default;
        stra.targetRoadNumber = env.roadNumber;
        switch (m_AiType)
        {
            case EAiType.Conservative:
                m_ConservativeAi.updateThreshold(m_WarnDistance, m_DynamicSafeDistance);
                stra = m_ConservativeAi.tick(in env);
                break;
            case EAiType.Radical:
                // stra = computeRaidcalAi(in prevStraitegic, in env);
                break;
        }

        return stra;
    }

    protected Vector3 generateAction(Strategic stra)
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
            var env = computeEnvironment();
            var stra = computeStrategic(env);
            var action = generateAction(stra);
            m_Controller.updateUserInput(action);
        }
    }

    // 根据当前所在位置和目标位置的百分比
    // 生成此时的目标转向在水平轴上的投影数值
    float computeTargetProjection(float offset)
    {
        var width = m_AttachRoad.getRoadWidth();
        var percent = offset / width; // -1 ～ 0 ～ 1
        percent = Mathf.Clamp(percent, -1f, 1f);
        return percent;
    }

    // 传入在水平轴上的投影，得到目标转向角度
    float computeTurnPercent(float offset)
    {
        var target = computeTargetProjection(offset);
        var current = computeCurrentTurnProject();   // -1 ~ 0 ~ 1

        var percent = target - current;
        // 加快回正速度
        if (percent * current < 0f)
        {
            percent *= 4f;
        }
        else
        {
            percent /= 4f;
        }
        percent = Mathf.Clamp(percent, -1f, 1f);

        return percent;
    }

    // 计算当前转向角度在水平轴上的投影
    float computeCurrentTurnProject()
    {
        var selfDirection = transform.TransformDirection(Vector3.forward);
        var current = m_AttachRoad.getDegreeProjection(selfDirection);
        return current;
    }

    void OnCollisionEnter(Collision info)
    {
        if (((1 << info.gameObject.layer) & m_ObstructLayer.value) != 0)
        {
        }
    }

    void OnDrawGizmos()
    {
        var origin = transform.position;

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
    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmosSelected(CarAiPawn script, GizmoType type)
    {

        var center = script.transform.position;
        // Draw Safe Circle
        var safe = script.getSafeDistance();
        var radius = 3f;
        var halfExt = Vector3.zero;
        halfExt.y = 0.2f;
        halfExt.x = radius * 3f;
        halfExt.z = safe * 2f;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, halfExt);

        var warn = script.getWarnDistance();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, warn);
    }
}
