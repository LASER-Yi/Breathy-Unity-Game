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

        m_Game.setClock(7f);
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
                m_Sound.playBtnEffect();

                var introText = DataManager.LoadTextSequence("Text/Menu");
                var param = LGameplay.SequenceTextParams.computeParamsFromString(introText, new UnityAction(setupMenuInterface));
                m_CanvasController.setupFullScreenText(param);
            }
        }
    }

    void setupMenuInterface()
    {
        if(m_SceneUiController is MenuUiController mu){
            mu.cleanBtnContainer();
            mu.setupButton("START GAME", new UnityAction(startGame));
            mu.setupButton("ABOUT", new UnityAction(aboutGame));
            mu.setupButton("EXIT GAME", new UnityAction(exitGame));
            mu.hideAnyText();
        }
        
        StartCoroutine(ieTransferOnStartup());
    }

    private void aboutGame(){
        if(m_SceneUiController is MenuUiController mu){
            mu.showAboutScreen();
        }
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
        target.setPosition(Vector3.up * 65f);
        target.setRotation(Quaternion.Euler(-5f, 0f, 0f));
        target.setZLength(250f);
        target.setFov(60f);

        m_CamController.startShakeCamera(1, 0.8f);
        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 4.5f);

        yield return new WaitForSeconds(1f);

        CameraAttribute next = CameraAttribute.Empty;
        next.setPosition(Vector3.up * 15f);
        next.setRotation(Quaternion.identity);
        next.setFov(17f);

        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(next, 0.7f);

    }

}
