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
        CameraAttribute attr = CameraAttribute.getEmpty().
        setPosition(Vector3.zero).setRotation(Quaternion.Euler(90f, 0f, 0f)).setZLength(20f).setFov(90f);

        m_Controller.setAttribute(attr);
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
