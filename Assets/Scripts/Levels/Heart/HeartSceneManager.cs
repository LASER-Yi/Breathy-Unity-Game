using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSceneManager : SceneBaseController
{

    private float m_CurrentHeartBpm;
    private float m_CurrentBreathBpm;

    /* Control variable, Range(0 ~ 1) */
    private float m_CurrentHeartPosition;
    private float m_CurrentHeartCursor;
    private float m_CurrentBreathPosition;
    private float m_CurrentBreathCursor;

    void setupCamera()
    {
        CameraAttribute attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.zero);
        attr.setRotation(Quaternion.Euler(90f, 0f, 0f));
        attr.setZLength(20f);
        attr.setFov(90f);

        m_CamController.setAttribute(attr);
    }
    new void Start()
    {
        base.Start();
        setupCamera();
    }

    void updateHeartState()
    {

    }

    void updateBreathState()
    {

    }
}
