using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameStructure;

public class RoadSceneManager : SceneBaseController
{
    private CarController m_PlayerCar;
    [SerializeField]
    private GameObject m_EndPoint;

    private RoadSceneParam m_SceneParam;

    private CameraFrameController m_FrameController;

    void Awake()
    {
        m_SceneParam = m_Game.computeRoadParam();
        m_FrameController = GetComponent<CameraFrameController>();
    }
    new void Start()
    {
        base.Start();
        m_PlayerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        m_PlayerCar.setEnginePower(0.4f);

        m_Game.setDeltaClock(0.5f);
        m_Game.setTimeSpeed(18f);
        m_Game.startTimeLoop();

        m_FrameController.enableFollow();
    }

    public float getPressForce()
    {
        return m_SceneParam.pressForce;
    }

    public float getPlayerVelocity()
    {
        return m_PlayerCar.getVelocity();
    }

    public float getDistanceToEndPoint()
    {
        return Vector3.Distance(m_PlayerCar.transform.position, m_EndPoint.transform.position);
    }

    private bool m_IsTriggerEndPoint = false;
    void Update()
    {
        if (getDistanceToEndPoint() < 20f && !m_IsTriggerEndPoint)
        {
            handleCarReachDestination();
            m_IsTriggerEndPoint = true;
        }
    }

    void handleCarReachDestination()
    {
        StartCoroutine(ieCarReachCoro());
    }

    IEnumerator ieCarReachCoro(){
        m_FrameController.disableFollow();
        yield return new WaitForSeconds(1f);
        GSceneController.instance.LoadNextScene(true);
    }
}
