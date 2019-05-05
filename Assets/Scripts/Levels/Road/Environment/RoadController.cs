using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// 管理路面的实例化和删除
// 在路面前方和后方放置Trigger, 用于移除AI车辆
public class RoadController : MonoBehaviour
{
    private RoadNode m_FirstRoad = null;    // 指向此路面区块链的头部（前方)
    private RoadNode m_LastRoad = null;     // 指向此路面区块链的尾部（后方）
    private IPawnController m_Player;

    [SerializeField]
    private int m_MaxRoadCount = 20;

    private int m_RoadCount = 0;

    [SerializeField]
    private GameObject m_ChunkPrefab;
    void Awake()
    {
        initalRoadChunk();
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<IPawnController>();
    }

    [ExecuteAlways]
    public void initalRoadChunk()
    {
        removeAllRoad();
        while (m_RoadCount < m_MaxRoadCount)
        {
            createRoadFront();
            createRoadRear();
        }
    }

    void Update(){
        if(m_Player != null && m_FirstRoad != null){
            var player = m_Player.getWorldPosition();
            var firstChunk = m_FirstRoad.transform.position;
            player.y = 0f;
            firstChunk.y = 0f;
            var dist = Vector3.Distance(player, firstChunk);
            var length = m_FirstRoad.getRoadLengthWorld();

            int chunk = Mathf.FloorToInt(dist / length);
            Debug.Log(chunk);
            if (chunk < Mathf.FloorToInt(m_RoadCount / 2f)){
                createRoadFront();
            }
        }
    }

    [ExecuteAlways]
    public void removeAllRoad()
    {
        foreach(Transform item in transform){
            Destroy(item.gameObject);
        }
    }

    public void createFirstRoad()
    {
        if (m_FirstRoad == null && m_LastRoad == null)
        {
            var position = transform.position;
            var rotation = transform.rotation;

            var obj = Instantiate(m_ChunkPrefab, position, rotation);
            var script = obj.GetComponent<RoadNode>();
            if (script != null)
            {
                obj.transform.SetParent(transform, true);
                m_FirstRoad = script;
                m_LastRoad = script;
                m_RoadCount = 1;
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    [ExecuteAlways]
    public void createRoadFront()
    {
        if (m_FirstRoad == null)
        {
            createFirstRoad();
        }
        if(m_RoadCount >= m_MaxRoadCount){
            removeRoadBehind();
        }
        m_FirstRoad = m_FirstRoad.createRoadFront(m_ChunkPrefab);
        ++m_RoadCount;
    }

    public void createRoadRear()
    {
        if (m_LastRoad == null)
        {
            createFirstRoad();
        }
        if(m_RoadCount < m_MaxRoadCount){
            m_LastRoad = m_LastRoad.createRoadBehide(m_ChunkPrefab);
            ++m_RoadCount;
        }
    }

    [ExecuteAlways]
    public void removeRoadBehind()
    {
        if (m_RoadCount > 1)
        {
            --m_RoadCount;
            var tempory = m_LastRoad;
            m_LastRoad = tempory.getFrontRoad();
            DestroyImmediate(tempory.gameObject);
        }
    }
}

[CustomEditor(typeof(RoadController))]
public class RoadControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = target as RoadController;

        if (GUILayout.Button("初始化"))
        {

        }

        if (GUILayout.Button("前方生成"))
        {
            script.createRoadFront();
        }

        if (GUILayout.Button("后方删除"))
        {
            script.removeRoadBehind();
        }
        if (GUILayout.Button("删除全部"))
        {
            script.removeAllRoad();
        }
    }
}
