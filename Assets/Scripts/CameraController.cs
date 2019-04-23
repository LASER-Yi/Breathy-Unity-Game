using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // [Controller]
    //  Position
    //      Rotation
    //          Camera
    private Transform m_RigPosition;
    private Transform m_RigRotation;
    private Transform m_RigCamera;
    private Camera m_Cam;

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
            m_RigCamera.localRotation = Quaternion.Euler(0f, 0f, value);
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
            m_RigCamera.localRotation = Quaternion.Euler(0f, 0f, euler.z);
        }
    }

    private Quaternion m_LocalRotation
    {
        set
        {
            Vector3 euler = value.eulerAngles;
            m_RigRotation.localRotation = Quaternion.Euler(euler.x, euler.y, 0f);
            m_RigCamera.localRotation = Quaternion.Euler(0f, 0f, euler.z);
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
            m_RigCamera.localPosition = Vector3.forward * value;
        }
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

    public void transferImmediate(Vector3 worldPosition, Quaternion worldRotation)
    {
        m_WorldPosition = worldPosition;
        m_WorldRotation = worldRotation;
    }

    public void transferImmediate(Vector3 worldPosition)
    {
        m_WorldPosition = worldPosition;
    }
    public void transferImmediate(Quaternion worldRotation)
    {
        m_WorldRotation = worldRotation;
    }

    public void transferImmediate(float zLength){
        m_CameraZLength = zLength;
    }

    public void transferInLerp()
    {
        
    }

    // 相机只在最后进行更新
    void LateUpdate(){

    }
}
