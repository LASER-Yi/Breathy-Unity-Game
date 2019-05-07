﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class SleepSceneManager : SceneBaseController
{
    private CameraController m_CamController
    {
        get
        {
            return LCameraSystem.CameraController.instance;
        }
    }
    new void Start()
    {
        base.Start();
        var attr = CameraAttribute.Empty;
        attr.rotation = Quaternion.Euler(70f, -90f, 0f);
        attr.position = new Vector3(-41f, 0f, 0f);
        attr.zlength = 105f;
        attr.fov = 60f;

        GameManager.instance.setTimeOfDay(21);
        GameManager.instance.startDayLoop();

        m_CamController.setAttribute(attr);
    }
}