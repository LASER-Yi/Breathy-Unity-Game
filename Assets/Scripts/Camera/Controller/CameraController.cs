using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 升级相机API为基于关键帧动画的API
// TODO: 使用name来进行动画分类

namespace LCameraSystem
{
    public class CameraController : MonoBehaviour
    {
        private static Object _lock = new Object();
        private static CameraController _instance;

        public static CameraController instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var obj = FindObjectOfType<CameraController>();
                        if (obj != null)
                        {
                            _instance = obj;
                        }
                    }
                }
                return _instance;
            }
        }
        // [Controller]
        //  Position
        //      Rotation
        //          Camera(with Roll)
        private Transform m_RigPosition;
        private Transform m_RigRotation;
        private Transform m_RigCamera;
        private Camera m_Cam;
        [SerializeField]
        private Camera m_UiCam;

        /* Controller */
        void Awake()
        {
            m_RigCamera = transform;
            m_Cam = GetComponent<Camera>();
            if (transform.parent != null)
            {
                m_RigRotation = m_RigCamera.parent;
                if (m_RigRotation.parent != null)
                {
                    m_RigPosition = m_RigRotation.parent;
                }
            }
        }

        public void startShakeCamera(int level, float time)
        {
            StartCoroutine(ieShakeCameraCoro(level, time));
        }

        private System.Object _shakeLock = new Object();

        IEnumerator ieShakeCameraCoro(int level, float time)
        {
            lock (_shakeLock)
            {
                float _level = (float)level / 5f;
                bool isOffset = false;
                float currentTime = 0f;
                while (currentTime < time)
                {
                    yield return null;
                    currentTime += Time.deltaTime;
                    if (isOffset)
                    {
                        setTrueCameraPosition(Vector2.zero);
                    }
                    else
                    {
                        var randX = Random.Range(-1f, 1f);
                        var randY = Random.Range(-1f, 1f);
                        var offset = new Vector2(randX, randY) * _level;
                        setTrueCameraPosition(offset);
                    }
                    isOffset = !isOffset;
                }
                setTrueCameraPosition(Vector2.zero);
            }
        }

        /* Setter */

        public void setAttribute(CameraAttribute attr)
        {
            if (attr.position is Vector3 pos) m_WorldPosition = pos;
            if (attr.rotation is Quaternion rot) m_WorldRotation = rot;
            if (attr.zlength is float length) m_CurrentZLength = length;
            if (attr.fov is float fov) m_CurrentFov = fov;
        }

        public void setAttribute(CameraAttributeNotNull attr)
        {
            m_WorldPosition = attr.position;
            m_WorldRotation = attr.rotation;
            m_CurrentZLength = attr.zlength;
            m_CurrentFov = attr.fov;
        }


        public void setPosition(Vector3 worldPosition)
        {
            m_WorldPosition = worldPosition;
        }
        public void setRotation(Quaternion worldRotation)
        {
            m_WorldRotation = worldRotation;
        }

        public void setZLength(float value)
        {
            m_CurrentZLength = value;
        }

        public void setFov(float value)
        {
            m_CurrentFov = value;
        }

        /* Getter */

        public CameraAttributeNotNull getAttribute()
        {
            var attr = new CameraAttributeNotNull();
            attr.position = m_WorldPosition;
            attr.rotation = m_WorldRotation;
            attr.zlength = m_CurrentZLength;
            attr.fov = m_CurrentFov;
            return attr;
        }

        public Camera getAttachCamera()
        {
            return m_Cam;
        }

        public float getFov()
        {
            return m_CurrentFov;
        }

        public Vector3 getWorldPosition()
        {
            return m_WorldPosition;
        }

        public Quaternion getWorldRotation()
        {
            return m_WorldRotation;
        }

        public float getZLength()
        {
            return m_CurrentZLength;
        }

        /* Attribute */

        private float m_RotatorPitchEuler
        {
            get
            {
                return m_RigRotation.rotation.eulerAngles.x;
            }
        }

        private float m_RotatorYawEuler
        {
            get
            {
                return m_RigRotation.rotation.eulerAngles.y;
            }
        }

        private float m_CameraRollEuler
        {
            get
            {
                return m_RigCamera.localRotation.eulerAngles.z;
            }
            set
            {
                var rotator = Quaternion.Euler(0f, 0f, value);
                m_RigCamera.localRotation = rotator;
                m_UiCam.transform.localRotation = rotator;
            }
        }

        private Vector3 m_WorldPosition
        {
            get
            {
                return m_RigPosition.position;
            }
            set
            {
                m_RigPosition.position = value;
            }
        }

        private Quaternion m_WorldRotation
        {
            get
            {
                return Quaternion.Euler(m_RotatorPitchEuler, m_RotatorYawEuler, m_CameraRollEuler);
            }
            set
            {
                Vector3 euler = value.eulerAngles;
                m_RigRotation.rotation = Quaternion.Euler(euler.x, euler.y, 0f);
                var rotator = Quaternion.Euler(0f, 0f, euler.z);
                m_RigCamera.localRotation = rotator;
                m_UiCam.transform.localRotation = rotator;
            }
        }

        private void setTrueCameraPosition(Vector2 position)
        {
            Vector3 pos = new Vector3(position.x, position.y, m_RigCamera.localPosition.z);
            m_RigCamera.transform.localPosition = pos;
            m_UiCam.transform.localPosition = pos;
        }

        private float m_CurrentZLength
        {
            get
            {
                return m_RigCamera.localPosition.z * -1f;
            }
            set
            {
                var local = m_RigCamera.localPosition;
                local.z = -value;
                m_RigCamera.localPosition = local;
                m_UiCam.transform.localPosition = local;
            }
        }

        private float m_CurrentFov
        {
            get
            {
                return m_Cam.fieldOfView;
            }
            set
            {
                m_Cam.fieldOfView = value;
                m_UiCam.fieldOfView = value;
            }
        }
    }
}
