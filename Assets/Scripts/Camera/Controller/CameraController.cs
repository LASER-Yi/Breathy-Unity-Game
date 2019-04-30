using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public delegate float interpolation(float t);

    private static float vSmoothStep(float t)
    {
        return Mathf.SmoothStep(0f, 1f, t);
    }

    public void moveCamera(CameraAttribute? from, CameraAttribute to, float time, interpolation func){
        if(from.HasValue) setAttribute(from.Value);

        StartCoroutine(ieTransCameraCoro(to, time, func));
    }

    private UnityEngine.Object intp_lock = new Object();

    public IEnumerator ieTransCameraCoro(CameraAttribute attr, float time, interpolation func)
    {
        lock (intp_lock)
        {
            float currentTime = 0f;
            float progress = 0f;

            var currPosition = m_WorldPosition;
            var currRotation = m_WorldRotation;
            var currLength = m_CurrentZLength;
            var currFov = m_CurrentFov;

            while (currentTime < time)
            {
                yield return new WaitForEndOfFrame();
                float deltaTime = Time.deltaTime;
                currentTime += deltaTime;
                progress = currentTime / time;

                float intp = func(progress);


                if (attr.position.HasValue) m_WorldPosition = Vector3.Lerp(currPosition, attr.position.Value, intp);
                if (attr.rotation.HasValue) m_WorldRotation = Quaternion.Slerp(currRotation, attr.rotation.Value, intp);
                if (attr.zlength.HasValue) m_CurrentZLength = Mathf.Lerp(currLength, attr.zlength.Value, intp);
                if (attr.fov.HasValue) m_CurrentFov = Mathf.Lerp(currFov, attr.fov.Value, intp);
            }
            if (attr.position.HasValue) m_WorldPosition = attr.position.Value;
            if (attr.rotation.HasValue) m_WorldRotation = attr.rotation.Value;
            if (attr.zlength.HasValue) m_CurrentZLength = attr.zlength.Value;
            if (attr.fov.HasValue) m_CurrentFov = attr.fov.Value;
            yield return null;
        }
    }

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

    /* Setter */

    public void setAttribute(CameraAttribute attr)
    {
        if (attr.position.HasValue) m_WorldPosition = attr.position.Value;
        if (attr.rotation.HasValue) m_WorldRotation = attr.rotation.Value;
        if (attr.zlength.HasValue) m_CurrentZLength = attr.zlength.Value;
        if (attr.fov.HasValue) m_CurrentFov = attr.fov.Value;
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

    public CameraAttributeNotNull getAttribute(){
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
