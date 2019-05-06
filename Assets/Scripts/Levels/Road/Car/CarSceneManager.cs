using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSceneManager : SceneBaseController
{
    private CarController m_PlayerCar;
    new void Start()
    {
        base.Start();
        m_PlayerCar = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();
        m_PlayerCar.setEnginePower(0.4f);
    }
}
