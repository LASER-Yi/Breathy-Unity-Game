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
    //          Camera
    private Transform m_RigPosition;
    private Transform m_RigRotation;
    private Transform m_RigCamera;
    private Camera m_Cam;
    [SerializeField]
    private Camera m_UiCam;

    public Camera getAttachCamera(){
        return m_Cam;
    }

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

    private Quaternion m_LocalRotation
    {
        set
        {
            Vector3 euler = value.eulerAngles;
            m_RigRotation.localRotation = Quaternion.Euler(euler.x, euler.y, 0f);
            var rotator = Quaternion.Euler(0f, 0f, euler.z);
            m_RigCamera.localRotation = rotator;
            m_UiCam.transform.localRotation = rotator;
        }
    }

    private float m_CameraZLength
    {
        get
        {
            return m_RigCamera.localPosition.z;
        }
        set
        {
            var zDistance = Vector3.back * value;
            m_RigCamera.localPosition = zDistance;
            m_UiCam.transform.localPosition = zDistance;
        }
    }

    public Vector3 getWorldPosition(){
        return m_WorldPosition;
    }

    public Quaternion getWorldRotation(){
        return m_WorldRotation;
    }

    public float getZLength(){
        return m_CameraZLength;
    }

    private Quaternion m_TargetWorldRotation;
    private Vector3 m_TargetWorldPosition;

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

    public void setFovOnCamera(float newFov){
        m_Cam.fieldOfView = newFov;
        m_UiCam.fieldOfView = newFov;
    }

    public void setTransform(Vector3 worldPosition, Quaternion worldRotation)
    {
        m_WorldPosition = worldPosition;
        m_WorldRotation = worldRotation;
    }

    public void setTransform(Vector3 worldPosition)
    {
        m_WorldPosition = worldPosition;
    }
    public void setTransform(Quaternion worldRotation)
    {
        m_WorldRotation = worldRotation;
    }

    public void setZLength(float zLength){
        m_CameraZLength = zLength;
    }
}
