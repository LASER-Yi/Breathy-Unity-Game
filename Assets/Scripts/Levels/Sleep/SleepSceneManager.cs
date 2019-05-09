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

    [SerializeField]
    private RectTransform m_ShopPanelPrefab;
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

        m_Game.setDeltaClock(0.1f);
        m_Game.startTimeLoop();
    }

    public void loadNextScene()
    {
        // 动画
        m_Game.increaseDayCount();
        GSceneController.instance.LoadNextScene(true);
    }

    public void clickShopBtn()
    {

    }

    public void clickSleepBtn()
    {
        StartCoroutine(iePrepareSleep());
    }

    IEnumerator iePrepareSleep()
    {
        m_SceneUiController.showWaitAction();
        yield return new WaitForSeconds(1f);
        m_RoomLight.enabled = false;
        yield return new WaitForSeconds(1f);
        yield return m_SceneUiController.transformStateBar();
        float waitTimer = m_Game.startLerpTime(8, 20);
        m_MoonLight.enabled = false;
        yield return new WaitForSeconds(waitTimer + 2);
        m_SceneUiController.showWakeupAction(this);

    }
}
