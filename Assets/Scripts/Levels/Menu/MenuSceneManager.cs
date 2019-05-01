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
        var attr = CameraAttribute.Empty;
        attr.setPosition(Vector3.up * 50f);
        attr.setRotation(Quaternion.identity);
        attr.setZLength(210f);
        attr.setFov(30f);

        CameraController.instance.setAttribute(attr);

        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 35f);
        target.setZLength(200f);

        yield return CameraAnimator.instance.ieStartCameraNextKeyframe(target, 0.6f, t => Mathf.SmoothStep(0f, 1f, t));

        m_IsBtnEnable = true;
    }

}
