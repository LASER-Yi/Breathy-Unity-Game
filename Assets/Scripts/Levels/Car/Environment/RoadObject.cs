using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CarLevel
{
    // 根据视角情况对路面进行初始化，在路后方生成车辆
    // 路恒定往+z方向生长
    public class RoadObject : MonoBehaviour
    {
        [SerializeField]
        private int m_RoadNum;
        [SerializeField]
        private float m_RoadWidth;
        [SerializeField]
        private GameObject m_RoadPrefab;

        public int getRoadNum()
        {
            return m_RoadNum;
        }
        public float getRoadWidth()
        {
            return m_RoadWidth;
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
    }

    [CustomEditor(typeof(RoadObject))]
    public class RoadControllerEditor : Editor
    {

        [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
        static void DrawGizmosSelected(RoadObject script, GizmoType type)
        {
            if (script.getRoadNum() == 0) return;

            float maxWidth = (script.getRoadNum() / 2.0f) * script.getRoadWidth();

            float drawLineLength = 20f;
            float heightOffset = 0.2f;
            Gizmos.DrawLine(script.transform.TransformPoint(new Vector3(-maxWidth, heightOffset, -drawLineLength / 2.0f)),
            script.transform.TransformPoint(new Vector3(-maxWidth, heightOffset, drawLineLength / 2.0f)));

            float maxOffset = maxWidth;
            for (int count = 0; count < script.getRoadNum(); ++count)
            {
                Gizmos.DrawLine(script.transform.TransformPoint(new Vector3(maxOffset, heightOffset, -drawLineLength / 2.0f)), script.transform.TransformPoint(new Vector3(maxOffset, heightOffset, drawLineLength / 2.0f)));

                maxOffset -= script.getRoadWidth();
            }

            Vector3 forward = script.transform.TransformDirection(Vector3.forward);
            Gizmos.DrawRay(new Ray(script.transform.position, forward));

        }
    }

}