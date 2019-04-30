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

    void setupCamera(){
        m_Controller.setTransform(Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
        m_Controller.setZLength(20f);
        m_Controller.setFov(90f);
    }
    void Start()
    {
        base.Start();
        setupCamera();
    }

    void updateHeartState(){

    }

    void updateBreathState(){

    }
}
