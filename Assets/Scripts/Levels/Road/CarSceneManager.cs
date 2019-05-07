using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSceneManager : SceneBaseController
{
    private CarController m_PlayerCar;
    [SerializeField]
    private GameObject m_EndPoint;
    new void Start()
    {
        base.Start();
        m_PlayerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        m_PlayerCar.setEnginePower(0.4f);

        GameManager.instance.setTimeOfDay(9);
        GameManager.instance.startDayLoop();
    }

    public float getPlayerVelocity(){
        return m_PlayerCar.getVelocity();
    }

    public float getDistanceToEndPoint(){
        return Vector3.Distance(m_PlayerCar.transform.position, m_EndPoint.transform.position);
    }
}
