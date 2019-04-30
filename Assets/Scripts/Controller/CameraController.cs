﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraAction
{
    public Vector3? ptarget;
    public Quaternion? rtarget;
    public float? zlength;
    public float? fov;

    public static CameraAction getEmpty(){
        return new CameraAction();
    }

    public CameraAction setPosition(Vector3 val)
    {
        ptarget = val;
        return this;
    }

    public CameraAction setRotation(Quaternion val)
    {
        rtarget = val;
        return this;
    }

    public CameraAction setZLength(float val)
    {
        zlength = val;
        return this;
    }

    public CameraAction setFov(float val)
    {
        fov = val;
        return this;
    }

}

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

    public delegate float interpolation(float t);

    private static float vSmoothStep(float t)
    {
        return Mathf.SmoothStep(0f, 1f, t);
    }

    private UnityEngine.Object intp_lock = new Object();

    public IEnumerator ieTransCameraCoro(CameraAction action, float time, interpolation func)
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

                if (action.ptarget.HasValue) m_WorldPosition = Vector3.Lerp(currPosition, action.ptarget.Value, intp);
                if (action.rtarget.HasValue) m_WorldRotation = Quaternion.Slerp(currRotation, action.rtarget.Value, intp);
                if (action.zlength.HasValue) m_CurrentZLength = Mathf.Lerp(currLength, action.zlength.Value, intp);
                if (action.fov.HasValue) m_CurrentFov = Mathf.Lerp(currFov, action.fov.Value, intp);
            }
            if (action.ptarget.HasValue) m_WorldPosition = action.ptarget.Value;
            if (action.rtarget.HasValue) m_WorldRotation = action.rtarget.Value;
            if (action.zlength.HasValue) m_CurrentZLength = action.zlength.Value;
            if (action.fov.HasValue) m_CurrentFov = action.fov.Value;
            yield return null;
        }
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

    public void setFov(float value)
    {
        m_CurrentFov = value;
    }

    public void setTransform(Vector3 worldPosition, Quaternion worldRotation)
    {
        m_WorldPosition = worldPosition;
        m_WorldRotation = worldRotation;
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
}
