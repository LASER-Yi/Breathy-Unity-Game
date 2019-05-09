﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LCameraSystem;

public class MenuSceneManager : SceneBaseController
{
    [SerializeField]
    private RectTransform m_SelectPrefab;

    private bool m_IsBtnEnable = false;
    private MenuUiController m_SceneUiController;

    public bool getButtonState()
    {
        return m_IsBtnEnable;
    }

    new void Start()
    {
        base.Start();
        m_SceneUiController = m_SceneUi.GetComponent<MenuUiController>();
        StartCoroutine(ieTransferOnStartup());
    }

    private bool m_IsAnyKeyDown = false;

    void Update(){
        if(!m_IsAnyKeyDown){
            if(Input.anyKeyDown){
                m_IsAnyKeyDown = true;
                setupMenuInterface();
            }
        }
    }

    void setupMenuInterface(){
        m_SceneUiController.cleanBtnContainer();
        m_SceneUiController.setupButton("开始游戏", new UnityAction(startGame));
        m_SceneUiController.setupButton("关于", null);
        m_SceneUiController.setupButton("结束游戏", new UnityAction(exitGame));
    }

    public void startGame()
    {
        GameManager.instance.setTimeOfDay(21f);
        GSceneController.instance.LoadNextScene(true);
    }

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator ieTransferOnStartup()
    {
        var lightController = GLightController.instance;
        GameManager.instance.setTimeOfDay(10f);

        var attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.down * 30f);
        attr.setRotation(Quaternion.Euler(-1f, 0f, 0f));
        attr.setZLength(100f);
        attr.setFov(17f);

        CameraController.instance.setAttribute(attr);

        yield return new WaitForSecondsRealtime(1f);

        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 10f);
        target.setRotation(Quaternion.Euler(2f, 0f, 0f));
        target.setZLength(250f);

        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 5f);

        m_IsBtnEnable = true;
    }

}
