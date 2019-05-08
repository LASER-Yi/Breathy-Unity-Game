using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class SleepSceneManager : SceneBaseController
{
    [SerializeField]
    private Light m_RoomLight;
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

        GameManager.instance.setTimeOfDay(21f);
        GameManager.instance.startTimeLoop();

        m_CamController.setAttribute(attr);

        m_SceneUiController = GCanvasController.instance.getCurrentRootUi().GetComponent<SleepMainUiController>();
        m_SceneUiController.showStartupAction();
    }

    public void loadNextScene(){
        // 动画
        GSceneController.instance.LoadSceneAsync(GSceneController.ESceneIndex.Road, true);
    }

    public void gotoSleep(){
        m_RoomLight.enabled = false;
        // 声音

    }

    IEnumerator prepareSleep(){
        m_RoomLight.enabled = false;
        yield return new WaitForSeconds(1f);
        
    }
}
