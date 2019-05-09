using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameDataStruct;

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

        GameManager.instance.setTimeDelta(0.5f);
        GameManager.instance.setTimeSpeed(18f);
        GameManager.instance.startTimeLoop();
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
