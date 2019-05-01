using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSceneManager : SceneBaseController
{

    private float m_CurrentHeartBpm;
    private float m_CurrentBreathBpm;

    /* Control variable, Range(0 ~ 1) */
    private bool m_IsHeartDown;
    private float m_CurrentHeartPosition;
    private float m_CurrentHeartCursor;

    private bool m_IsBreathDown;
    private float m_CurrentBreathPosition;
    private float m_CurrentBreathCursor;

    /* Target */
    private float m_TargetHeartBpm;
    private float m_TargetBreathBpm;

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

    float computeAffect(float position, float cursor, bool isDown){
        float affect = 1.0f;
        float dist = position - cursor;
        if(isDown){
            affect *= dist;
        }else {
            affect *= -dist;
        }
        return affect;
    }

    void updateHeartState()
    {
        var affect = computeAffect(m_CurrentHeartPosition, m_CurrentBreathCursor, m_IsHeartDown);
        m_CurrentHeartBpm += affect;
    }

    void updateBreathState()
    {
        var affect = computeAffect(m_CurrentBreathPosition, m_CurrentBreathCursor, m_IsBreathDown);
        m_CurrentBreathBpm += affect;
    }

    float computeDeltaFromBpm(float bpm){
        var perSecond = bpm / 60f;
        return perSecond * Time.deltaTime;
    }

    void updateBothPosition(){
        // m_CurrentHeartPosition 
    }

    void updateVisualEffect(){

    }

    IEnumerator ieSceneLoop(){
        yield return iePreGame();
        yield return ieMainGame();
        yield return ieEndGame();
    }

    IEnumerator iePreGame(){
        yield return null;
    }

    IEnumerator ieMainGame(){
        yield return null;
    }

    IEnumerator ieEndGame(){
        yield return null;
    }
}
