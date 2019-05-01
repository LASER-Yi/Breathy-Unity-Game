using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSceneManager : SceneBaseController
{

    private float m_CurrentHeartBpm;

    /* Control variable, Range(0 ~ 1) */
    private bool m_IsHeartDown;
    private float m_CurrentHeartPosition;
    private float m_CurrentHeartCursor;

    /* Target */
    private float m_TargetHeartBpm;

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
        StartCoroutine(ieSceneLoop());
    }

    float computeAffect(float position, float cursor, bool isDown)
    {
        float affect = 1.0f;
        float dist = position - cursor;
        if (isDown)
        {
            affect *= dist;
        }
        else
        {
            affect *= -dist;
        }
        return affect;
    }

    float computeDeltaFromBpm(float bpm)
    {
        var perSecond = bpm / 60f;
        return perSecond * Time.deltaTime;
    }

    void updateUserInput(){
        var isPress = Input.GetButton("Jump");
        if(isPress){
            m_CurrentHeartCursor += Time.deltaTime;
        }else{
            m_CurrentHeartCursor -= Time.deltaTime;
        }
        m_CurrentHeartCursor = Mathf.Clamp01(m_CurrentHeartCursor);
    }

    void updateHeartState()
    {
        var affect = computeAffect(m_CurrentHeartPosition, m_CurrentHeartCursor, m_IsHeartDown);
        m_CurrentHeartBpm += affect;
    }

    void updateHeartPosition()
    {
        var delta = computeDeltaFromBpm(m_CurrentHeartBpm);
        if (m_IsHeartDown)
        {
            m_CurrentHeartPosition -= delta;
        }
        else
        {
            m_CurrentHeartPosition += delta;
        }
        if(m_CurrentHeartPosition < 0f){
            m_IsHeartDown = false;
        }else if(m_CurrentHeartPosition > 1f){
            m_IsHeartDown = true;
        }
        m_CurrentHeartPosition = Mathf.Clamp01(m_CurrentHeartPosition);
    }

    void updateHeartLoop()
    {
        updateUserInput();
        updateHeartState();
        updateHeartPosition();
        updateVisualEffect();
    }

    void updateVisualEffect()
    {

    }

    IEnumerator ieSceneLoop()
    {
        yield return iePreGame();
        yield return ieMainGame();
        yield return ieEndGame();
    }

    IEnumerator iePreGame()
    {
        yield return null;
    }

    IEnumerator ieMainGame()
    {
        // TODO: How game end
        while (true)
        {
            updateHeartLoop();
        }
    }

    IEnumerator ieEndGame()
    {
        yield return null;
    }
}
