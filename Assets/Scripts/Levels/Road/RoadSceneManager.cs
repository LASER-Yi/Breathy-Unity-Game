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
    new void Start()
    {
        base.Start();
        m_PlayerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        m_PlayerCar.setEnginePower(0.4f);

        m_Game.setDeltaClock(0.5f);
        m_Game.setTimeSpeed(18f);
        m_Game.startTimeLoop();
    }

    public float getPlayerVelocity()
    {
        return m_PlayerCar.getVelocity();
    }

    public float getDistanceToEndPoint()
    {
        return Vector3.Distance(m_PlayerCar.transform.position, m_EndPoint.transform.position);
    }

    void Update()
    {
        if (getDistanceToEndPoint() < 20f)
        {
            handleCarReachDestination();
        }
    }

    void handleCarReachDestination()
    {
        loadNextScene();
    }

    void loadNextScene()
    {
        GSceneController.instance.LoadNextScene(true);
    }
}
