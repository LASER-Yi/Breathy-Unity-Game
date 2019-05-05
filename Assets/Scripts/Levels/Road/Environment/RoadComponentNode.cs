using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 维护两个区块的统一状态
// 生成AI车辆
public class RoadComponentNode : MonoBehaviour
{
    private static int count = 0;
    private RoadComponentNode m_FrontRoad = null;          // 此路面的前一区块
    private RoadComponentNode m_BehideRoad = null;           // 此路面的后一区块

    [SerializeField]
    private float m_RoadLength;

    public float getRoadLength()
    {
        return m_RoadLength;
    }

    public RoadComponentNode getFrontRoad(){
        return m_FrontRoad;
    }

    void Awake()
    {
        ++count;
        gameObject.name = "Road_" + count;
    }

    public RoadComponentNode createRoadFront(GameObject prefab)
    {
        if (m_FrontRoad == null)
        {
            var nextPosition = transform.TransformPoint(Vector3.forward * m_RoadLength);
            var nextRotation = transform.rotation;

            var obj = Instantiate(prefab, nextPosition, nextRotation);
            var script = obj.GetComponent<RoadComponentNode>();
            if (script != null)
            {
                obj.transform.SetParent(transform.parent, true);
                m_FrontRoad = script;
                script.setBackRoad(this);
            }
            else
            {
                Debug.LogAssertion("生成的路没有绑定此代码");
            }
        }
        return m_FrontRoad;
    }

    public RoadComponentNode createRoadBehide(GameObject prefab)
    {
        if (m_BehideRoad == null)
        {
            var prevPosition = transform.TransformPoint(Vector3.back * m_RoadLength);
            var prevRotation = transform.rotation;

            var obj = Instantiate(prefab, prevPosition, prevRotation);
            var script = obj.GetComponent<RoadComponentNode>();
            if (script != null)
            {
                obj.transform.SetParent(transform.parent, true);
                m_BehideRoad = script;
                script.setFrontRoad(this);
            }
            else
            {
                Debug.LogAssertion("生成的路没有绑定此代码");
            }
        }
        return m_BehideRoad;
    }

    protected void setBackRoad(RoadComponentNode node)
    {
        if (m_BehideRoad == null)
        {
            m_BehideRoad = node;
        }
        else
        {
            Debug.LogAssertion("重复绑定BackRoad!!");
        }
    }

    public void setFrontRoad(RoadComponentNode node){
        if (m_FrontRoad == null)
        {
            m_FrontRoad = node;
        }
        else
        {
            Debug.LogAssertion("重复绑定FrontRoad!!");
        }
    }

}

[CustomEditor(typeof(RoadComponentNode))]
public class RoadComponentEditor : Editor
{

    [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
    static void DrawGizmosSelected(RoadComponentNode script, GizmoType type)
    {
        // Draw road m_RoadLength

        var center = script.transform.position;
        var length = script.getRoadLength() / 2f;
        var width = 4f;

        var frontLeft = script.transform.TransformPoint((Vector3.left * width + Vector3.forward * length));
        var frontRight = script.transform.TransformPoint((Vector3.right * width + Vector3.forward * length));

        Gizmos.DrawLine(frontLeft, frontRight);
    }
}
