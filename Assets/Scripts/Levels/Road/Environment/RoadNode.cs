using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 维护两个区块的统一状态
// 生成AI车辆
public class RoadNode : MonoBehaviour
{
    private static int count = 0;
    private RoadNode m_FrontRoad = null;          // 此路面的前一区块
    private RoadNode m_BehideRoad = null;           // 此路面的后一区块

    private List<RoadChunk> m_Chunks;

    [SerializeField]
    private float m_RoadLength;

    public float getRoadLength()
    {
        return m_RoadLength;
    }

    public RoadChunk getRandomChunk(){
        var picker = Random.Range(0, m_Chunks.Count);
        return m_Chunks[picker];
    }

    public float getRoadLengthWorld(){
        return m_RoadLength * transform.lossyScale.z;
    }

    public RoadNode getFrontRoad(){
        return m_FrontRoad;
    }

    void Awake()
    {
        ++count;
        gameObject.name = "Road_" + count;
        m_Chunks = new List<RoadChunk>(GetComponentsInChildren<RoadChunk>());
    }

    public RoadNode createRoadFront(GameObject prefab)
    {
        if (m_FrontRoad == null)
        {
            var nextPosition = transform.TransformPoint(Vector3.forward * m_RoadLength);
            var nextRotation = transform.rotation;

            var obj = Instantiate(prefab, nextPosition, nextRotation);
            var script = obj.GetComponent<RoadNode>();
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

    public RoadNode createRoadBehide(GameObject prefab)
    {
        if (m_BehideRoad == null)
        {
            var prevPosition = transform.TransformPoint(Vector3.back * m_RoadLength);
            var prevRotation = transform.rotation;

            var obj = Instantiate(prefab, prevPosition, prevRotation);
            var script = obj.GetComponent<RoadNode>();
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

    protected void setBackRoad(RoadNode node)
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

    public void setFrontRoad(RoadNode node){
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

#if UNITY_EDITOR
[CustomEditor(typeof(RoadNode))]
public class RoadComponentEditor : Editor
{

    [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
    static void DrawGizmosSelected(RoadNode script, GizmoType type)
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
#endif