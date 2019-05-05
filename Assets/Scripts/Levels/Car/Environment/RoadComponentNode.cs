using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 负责控制路面的延展和释放
public class RoadComponentNode : MonoBehaviour
{
    private static int count = 0;
    private static RoadComponentNode m_FirstRoad;    // 指向此路面区块链的头部（前方)
    private static RoadComponentNode m_LastRoad;     // 指向此路面区块链的尾部（后方）

    public static RoadComponentNode getFirstChunk()
    {
        return m_FirstRoad;
    }

    public static RoadComponentNode getLastChunk()
    {
        return m_LastRoad;
    }
    private RoadComponentNode m_FrontRoad = null;          // 此路面的前一区块
    private RoadComponentNode m_BackRoad = null;           // 此路面的后一区块

    [SerializeField]
    private float m_RoadLength;

    public float getRoadLength()
    {
        return m_RoadLength;
    }
    private static GameObject m_RoadChunkPrefab;

    private Dictionary<RoadChunk, bool> m_RoadObjectsWithVisible = new Dictionary<RoadChunk, bool>();

    void Awake()
    {
        var roads = GetComponentsInChildren<RoadChunk>();
        foreach (var item in roads)
        {
            item.tieComponent(this);
            m_RoadObjectsWithVisible.Add(item, true);
        }
        ++count;
        gameObject.name = "Road_" + count;
    }

    public void updateRoadVisible(RoadChunk road, bool visible)
    {
        if (m_RoadObjectsWithVisible.ContainsKey(road))
        {
            m_RoadObjectsWithVisible[road] = visible;
            if (visible)
            {
                generateBothRoad();
            }
        }else{
            Debug.LogAssertion("查找了不存在的路!!!");
        }
    }

    public void generateBothRoad()
    {
        if(m_RoadChunkPrefab == null){
            m_RoadChunkPrefab = Resources.Load<GameObject>("Levels/Road/Prefabs/Road Chunk");
        }
        generateRoadFront();
    }

    private static UnityEngine.Object generate_lock = new Object();

    void generateRoadFront()
    {
        lock (generate_lock)
        {
            if (m_FrontRoad == null)
            {
                var nextPosition = transform.position;
                nextPosition += Vector3.forward * m_RoadLength;
                var nextRotation = transform.rotation;

                var obj = Instantiate(m_RoadChunkPrefab, nextPosition, nextRotation);
                var script = obj.GetComponent<RoadComponentNode>();
                if (script != null)
                {
                    obj.transform.SetParent(transform.parent, true);
                    m_FrontRoad = script;
                    script.setBackInstance(this);
                    RoadComponentNode.m_FirstRoad = script;
                }
                else
                {
                    Debug.LogAssertion("生成的路没有绑定此代码");
                }
            }
        }
    }

    protected void setBackInstance(RoadComponentNode script)
    {
        if (m_BackRoad == null)
        {
            m_BackRoad = script;
        }
        else
        {
            Debug.LogAssertion("重复绑定BackRoad!!");
        }
    }

    bool checkIsAllAiRemain()
    {
        foreach (var item in m_RoadObjectsWithVisible)
        {
            if (item.Key.getAiCount() > 0)
            {
                return true;
            }
        }
        return false;
    }

    bool checkAllVisible()
    {
        foreach (var item in m_RoadObjectsWithVisible)
        {
            if (item.Value)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (!checkRoadAvaliable() && Time.time != 0f)
        {
            // 先每一帧检查 
            // TODO: 性能优化
            RoadComponentNode.m_LastRoad = m_FrontRoad;
            Destroy(gameObject);
        }
    }

    bool checkRoadAvaliable()
    {
        var isBackRoadAvaliable = (m_BackRoad != null);
        var isVisiable = checkAllVisible();
        var isAiExist = checkIsAllAiRemain();
        return isBackRoadAvaliable || isVisiable || isAiExist;
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
        var length = script.getRoadLength();
        var width = 20f;

        var frontLeft = center + (Vector3.left * width + Vector3.forward * length) / 2f;
        var frontRight = center + (Vector3.right * width + Vector3.forward * length) / 2f;

        Gizmos.DrawLine(frontLeft, frontRight);
    }
}
