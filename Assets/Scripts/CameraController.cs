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

    private Quaternion m_WorldRotation
    {
        get
        {
            return m_RigRotation.rotation;
        }
        set
        {
            m_RigRotation.rotation = value;
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
            m_RigPosition.SetPositionAndRotation(value, Quaternion.identity);
        }
    }

    private float m_CamBridgeLength
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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
