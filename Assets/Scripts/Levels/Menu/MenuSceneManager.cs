using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LCameraSystem;

public class MenuSceneManager : SceneBaseController
{

    new void Start()
    {
        base.Start();
        m_Game.resetWholeGame();

        m_Game.setClock(10f);
        var attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.down * 30f);
        attr.setRotation(Quaternion.Euler(-1f, 0f, 0f));
        attr.setZLength(100f);
        attr.setFov(17f);

        m_CamController.setAttribute(attr);
    }

    private bool m_IsAnyKeyDown = false;

    void Update()
    {
        if (!m_IsAnyKeyDown)
        {
            if (Input.anyKeyDown)
            {
                m_IsAnyKeyDown = true;
                var param = new LGameplay.SequenceTextParams("车辆");
                param.concat("合格")
                .concat("收入").concat("合格")
                .concat("工作资历").concat("合格")
                .concat("恭喜您通过了我们的审核，获得在雾之城中央财务处工作的资格")
                .concat("您在云上XH-90214的房间已可以入住")
                .concat("请明天上午9点前往云顶办公大楼报道")
                .concat("雾之城云上人力资源处 | 2059.00.30", null, null, new UnityAction(setupMenuInterface));
                m_CanvasController.setupFullScreenText(param);
            }
        }
    }

    void setupMenuInterface()
    {
        if(m_SceneUiController is MenuUiController mu){
            mu.cleanBtnContainer();
            mu.setupButton("开始游戏", new UnityAction(startGame));
            mu.setupButton("关于", null);
            mu.setupButton("结束游戏", new UnityAction(exitGame));
        }
        
        StartCoroutine(ieTransferOnStartup());
    }

    private void startGame()
    {
        m_Game.setClock(21f);
        GSceneController.instance.LoadNextScene(true);
    }

    private void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator ieTransferOnStartup()
    {
        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 10f);
        target.setRotation(Quaternion.Euler(2f, 0f, 0f));
        target.setZLength(250f);

        m_CamController.startShakeCamera(1, 0.8f);
        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 5f);
        
    }

}
