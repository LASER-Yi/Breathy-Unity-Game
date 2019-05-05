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

    // 根据给定索引计算路中央
    // 索引为路本地坐标+x方向增加
    public float computeRoadCenterWorld(int index)
    {
        float forward = transform.TransformDirection(Vector3.forward).z;
        forward = forward > 0 ? 1f : -1f;
        float centerZ = transform.position.z;
        float maxWidth = -(m_RoadNum / 2.0f) * m_RoadWidth;
        float positionX = maxWidth + ((m_RoadWidth / 2.0f) * (2 * index + 1));
        return transform.position.x + positionX;
    }

    // 根据给定位置计算路索引
    // 给定位置为世界坐标
    public int computeRoadNumberWorld(float worldPosition)
    {
        float offset = worldPosition - transform.position.x;
        offset += (m_RoadNum * m_RoadWidth) / 2f;

        int number = Mathf.FloorToInt(offset / m_RoadWidth);
        if (number < m_RoadNum && number >= 0)
        {
            return number;
        }
        else
        {
            return -1;
        }
    }

    public Vector3 getForwardDirection()
    {
        return transform.TransformDirection(Vector3.forward);
    }

    // 位置在路面上的百分比投影
    // -1 - 0  - 1
    // LR - CR - RR
    public float getHorizonalProject(Vector3 direction)
    {
        var right = transform.TransformDirection(Vector3.right);
        return Vector3.Project(direction, right).x / m_RoadWidth;
    }

    // 方向在路面上的左右投影
    // -1 - 0 - 1
    // L - M - R
    public float getDegreeProjection(Vector3 direction)
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

        float drawLineLength = 20f;
        float heightOffset = 0.2f;

        Gizmos.DrawLine(original + new Vector3(-maxWidth, heightOffset, -drawLineLength / 2.0f),
        original + new Vector3(-maxWidth, heightOffset, drawLineLength / 2.0f));

        float maxOffset = maxWidth;
        for (int count = 0; count < script.getRoadNum(); ++count)
        {
            Gizmos.DrawLine(original + new Vector3(maxOffset, heightOffset, -drawLineLength / 2.0f),
             original + new Vector3(maxOffset, heightOffset, drawLineLength / 2.0f));

            maxOffset -= script.getRoadWidth();
        }

        Vector3 forward = script.transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(new Ray(script.transform.position, forward));
    }
}