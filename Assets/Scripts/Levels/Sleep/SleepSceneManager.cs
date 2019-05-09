using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class SleepSceneManager : SceneBaseController
{
    [SerializeField]
    private Light m_RoomLight;
    [SerializeField]
    private Light m_MoonLight;
    private CameraController m_CamController
    {
        get
        {
            return LCameraSystem.CameraController.instance;
        }
    }

    private SleepMainUiController m_SceneUiController;
    new void Start()
    {
        base.Start();
        var attr = CameraAttribute.Empty;
        attr.rotation = Quaternion.Euler(70f, -90f, 0f);
        attr.position = new Vector3(-41f, 0f, 0f);
        attr.zlength = 105f;
        attr.fov = 60f;

        m_CamController.setAttribute(attr);

        m_SceneUiController = m_SceneUi.GetComponent<SleepMainUiController>();
        m_SceneUiController.showStartupAction(this);

        GameManager.instance.setTimeDelta(0.1f);
        GameManager.instance.startTimeLoop();
    }

    public void loadNextScene(){
        // 动画
        GameManager.instance.addDayCount();
        GSceneController.instance.LoadNextScene(true);
    }

    public void clickSleepBtn(){
        StartCoroutine(iePrepareSleep());
    }

    IEnumerator iePrepareSleep(){
        m_SceneUiController.showWaitAction();
        yield return new WaitForSeconds(1f);
        m_RoomLight.enabled = false;
        yield return new WaitForSeconds(1f);
        yield return m_SceneUiController.transformStateBar();
        float waitTimer = GameManager.instance.startLerpTime(8, 20);
        m_MoonLight.enabled = false;
        yield return new WaitForSeconds(waitTimer + 2);
        m_SceneUiController.showWakeupAction(this);

    }
}
