using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// 与车辆AI进行通信
public class RoadChunk : MonoBehaviour
{

    [SerializeField]
    private int m_RoadNum;
    [SerializeField]
    private float m_RoadWidth;
    private int m_AboveCarCount = 0;
    private RoadComponentNode m_Component = null;

    public float getRoadWidth()
    {
        return m_RoadWidth;
    }

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Road");
        m_Component = GetComponent<RoadComponentNode>();
    }

    /* Component 通信 */

    public int getAiCount()
    {
        return m_AboveCarCount;
    }


    /* Ai 通信 */

    public int getRoadNum()
    {
        return m_RoadNum;
    }

    // 根据给定索引 -> 计算路中央坐标
    public Vector3 computeRoadCenterWorld(int index, Vector3 worldPos)
    {

        // float maxWidth = -(m_RoadNum / 2f);
        float minRoadCenter = ((-m_RoadNum / 2f) + 0.5f) * m_RoadWidth;
        minRoadCenter += index * m_RoadWidth;

        Vector3 worldPoint = transform.TransformPoint(Vector3.right * minRoadCenter);
        return worldPos + worldPoint;
    }

    public float computeRoadCenterLocalOffset(int index){
        float minRoadCenter = ((-m_RoadNum / 2f) + 0.5f) * m_RoadWidth;
        minRoadCenter += index * m_RoadWidth;
        return minRoadCenter;
    }

    public float computeRoadCenterOffset(int index, Vector3 worldPos){
        var local = transform.InverseTransformPoint(worldPos);
        var target = computeRoadCenterLocalOffset(index);
        return local.x - target;
    }

    // 根据给定位置计算路索引
    // 给定位置为世界坐标
    public int computeRoadNumberWorld(Vector3 worldPosition)
    {
        var local = transform.InverseTransformPoint(worldPosition).x;
        var offset = (m_RoadNum * m_RoadWidth) / 2f;
        local += offset;

        int index = Mathf.FloorToInt(local / m_RoadWidth);
        if (index < m_RoadNum && index >= 0)
        {
            return index;
        }
        else
        {
            return -1;
        }
    }

    // 方向在路面上的左右投影
    // -1 - 0 - 1
    // L - M - R
    public float computeDegreeProjection(Vector3 direction)
    {
        var right = transform.TransformDirection(Vector3.right);
        return Vector3.Dot(direction, right);
    }

    public bool isRoadAvaliable(int roadNum)
    {
        int maxRange = m_RoadNum;
        // 与路面通信检查状况
        return roadNum >= 0 && roadNum < maxRange;
    }

    public void addCarToRoad()
    {
        ++m_AboveCarCount;
    }

    public void removeCarFromRoad()
    {
        --m_AboveCarCount;
    }
}

[CustomEditor(typeof(RoadChunk))]
public class RoadObjectEditor : Editor
{

    [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
    static void DrawGizmosSelected(RoadChunk script, GizmoType type)
    {
        if (script.getRoadNum() == 0) return;

        var original = script.transform.position;

        float maxWidth = (script.getRoadNum() / 2.0f) * script.getRoadWidth();

        float drawLineLength = 5f;
        float heightOffset = 0.2f;

        var button = script.transform.TransformPoint(new Vector3(-maxWidth, heightOffset, -drawLineLength / 2.0f));

        var top = script.transform.TransformPoint(new Vector3(-maxWidth, heightOffset, drawLineLength / 2.0f));

        Gizmos.DrawLine(button, top);

        float maxOffset = maxWidth;
        for (int count = 0; count < script.getRoadNum(); ++count)
        {

            var _button = script.transform.TransformPoint(new Vector3(maxOffset, heightOffset, -drawLineLength / 2.0f));

            var _top = script.transform.TransformPoint(new Vector3(maxOffset, heightOffset, drawLineLength / 2.0f));

            Gizmos.DrawLine(_button, _top);

            maxOffset -= script.getRoadWidth();
        }

        Vector3 forward = script.transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(new Ray(script.transform.position, forward));
    }
}