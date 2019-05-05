using System.Collections;
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

    public void gotoSelectGame()
    {
        if (!m_IsBtnEnable) return;
        m_UiController.pushToStack(m_SelectPrefab, true).GetComponent<SelectUiController>().setupManager(this);
    }

    public void loadSubGame(GSceneController.ESceneIndex index)
    {
        if (!m_IsBtnEnable) return;
        StopAllCoroutines();
        GSceneController.instance.LoadSceneAsync(index);
    }

    IEnumerator ieTransferOnStartup()
    {
        var lightController = GLightController.instance;
        lightController.setTimeOfDays(6f);
        lightController.startDefaultLightLoop();

        var attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.down * 30f);
        attr.setRotation(Quaternion.Euler(0f, 30f, 0f));
        attr.setZLength(400f);
        attr.setFov(17f);

        CameraController.instance.setAttribute(attr);

        yield return new WaitForSecondsRealtime(0.6f);

        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 20f);
        target.setRotation(Quaternion.identity);
        target.setZLength(300f);

        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 2f, t => Mathf.SmoothStep(0f, 1f, t));

        m_IsBtnEnable = true;
    }

}
