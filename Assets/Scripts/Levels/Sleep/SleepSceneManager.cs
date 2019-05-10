using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;
using LGameStructure;

public class SleepSceneManager : SceneBaseController, ITimeDidChangedHandler
{
    private SleepSceneParam m_Param;
    [SerializeField]
    private Light m_RoomLight;
    [SerializeField]
    private Light m_MoonLight;
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

        m_Game.setTimeSpeed(24f);
        m_Game.setDeltaClock(0.1f);
        m_Game.startTimeLoop();

        m_Param = m_Game.computeSleepParam();
    }

    public void loadNextScene()
    {
        // 动画
        m_Game.increaseDayCount();
        GSceneController.instance.LoadNextScene(true);
    }

    public void clickShopBtn()
    {
        m_SceneUiController.setupShopPanel(null, m_Game.getCharacterData().coin);
    }

    public void clickSleepBtn()
    {
        StartCoroutine(ieSleepProcess());
    }

    public void OnGameTimeChanged(GameManager sender, LGameStructure.TimeOfGame time){
        if(time.hour < 17 || time.hour > 5){
            m_MoonLight.enabled = false;
        }else{
            m_MoonLight.enabled = true;
        }
    }

    IEnumerator ieSleepProcess()
    {
        m_SceneUiController.showWaitAction();
        yield return new WaitForSeconds(1f);
        m_RoomLight.enabled = false;
        yield return new WaitForSeconds(1f);

        yield return m_SceneUiController.transformStateBar();
        float currentTimer = 0f;
        float waitTimer = m_Game.startLerpTime(8, 20);

        float recoverRate = 1f / waitTimer;

        while (currentTimer < waitTimer)
        {
            yield return null;
            currentTimer += Time.deltaTime;

            m_Game.increaseShieldValue(Time.deltaTime *
             recoverRate * m_Param.shieldRecoverRate);
        }
        m_SceneUiController.showWakeupAction(this);

    }
}
