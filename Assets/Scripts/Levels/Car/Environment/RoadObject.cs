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
        private static int count = 0;

        [SerializeField]
        private int m_RoadNum;
        [SerializeField]
        private float m_RoadWidth;
        [SerializeField]
        private float m_RoadLength;

        [Space, SerializeField]
        private GameObject m_RoadPrefab;
        private RoadObject m_FrontRoad;
        private RoadObject m_BackRoad;
        private bool m_IsVisible;
        private HashSet<CarAiPawn> m_AboveAiCars;

        public int getRoadNum()
        {
            return m_RoadNum;
        }
        public float getRoadWidth()
        {
            return m_RoadWidth;
        }

        public float getRoadLength()
        {
            return m_RoadLength;
        }

        void Start(){
            ++count;
            gameObject.name = "Road_" + count;
            m_IsVisible = true;
            StartCoroutine(ieCheckRoadState());
        }

        protected void setBackInstance(RoadObject script){
            if(m_BackRoad == null){
                m_BackRoad = script;
            }else{
                Debug.LogAssertion("重复绑定BackRoad!!");
            }
        }

        /* Ai 通信 */

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

        public void addAiToRoad(CarAiPawn ai){
            m_AboveAiCars.Add(ai);
        }

        public void removeAiFromRoad(CarAiPawn ai){
            m_AboveAiCars.Remove(ai);
        }

        void generateRoadFront(){
            if (m_FrontRoad == null)
            {
                var nextPosition = transform.position;
                nextPosition += Vector3.forward * m_RoadLength;
                var nextRotation = transform.rotation;
                var obj = Instantiate(m_RoadPrefab, nextPosition, nextRotation);
                var script = obj.GetComponent<RoadObject>();
                if (script != null)
                {
                    obj.transform.SetParent(transform.parent, true);
                    m_FrontRoad = script;
                    script.setBackInstance(this);
                }
                else
                {
                    Debug.LogAssertion("生成的路没有绑定RoadObject");
                }
            }
        }

        bool checkRoadAvaliablity(){
            var isBackRoadAvaliable = (m_BackRoad != null);
            var isVisiable = m_IsVisible;
            var isAiExist = (m_AboveAiCars.Count != 0);
            return isBackRoadAvaliable || isVisiable || isAiExist;
        }

        void OnBecameVisible()
        {
            m_IsVisible = true;
            generateRoadFront();
        }

        void OnBecameInvisible()
        {
            m_IsVisible = false;
        }

        void Update(){
            if(!checkRoadAvaliablity()){
                Destroy(gameObject);
            }
        }

        IEnumerator ieCheckRoadState(){
            while (true)
            {
                yield return new WaitForSeconds(1f);
            }
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

            // Draw Road Length

            Vector3 frontLeft = new Vector3(-maxWidth, heightOffset, script.getRoadLength() / 2f);
            Vector3 frontRight = new Vector3(maxWidth, heightOffset, script.getRoadLength() / 2f);
            Gizmos.DrawLine(frontLeft, frontRight);
        }
    }

}