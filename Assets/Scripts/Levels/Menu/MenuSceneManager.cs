﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class MenuSceneManager : SceneBaseController
{
    [SerializeField]
    private RectTransform m_SelectPrefab;

    private bool m_IsBtnEnable = false;


    public bool getButtonState()
    {
        return m_IsBtnEnable;
    }

    new void Start()
    {
        base.Start();
        var menuController = m_SceneUi.GetComponent<MenuUiController>();
        if (menuController != null)
        {
            menuController.setManager(this);
        }
        StartCoroutine(ieTransferOnStartup());
    }

    public void startGame(){
        GSceneController.instance.LoadSceneAsync(GSceneController.ESceneIndex.Sleep, true);
    }

    IEnumerator ieTransferOnStartup()
    {
        var lightController = GLightController.instance;
        GameManager.instance.setTimeOfDay(10f);

        var attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.down * 30f);
        attr.setRotation(Quaternion.Euler(0f, 10f, 0f));
        attr.setZLength(400f);
        attr.setFov(17f);

        CameraController.instance.setAttribute(attr);

        yield return new WaitForSecondsRealtime(0.6f);

        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 20f);
        target.setRotation(Quaternion.Euler(45f, -30f, 0));
        target.setZLength(500f);

        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 4f);

        m_IsBtnEnable = true;
    }

}
