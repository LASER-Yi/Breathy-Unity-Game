using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneManager : SceneBaseController
{
    [SerializeField]
    private RectTransform m_SelectPrefab;

    private float m_RotateSpeed = 10f;
    private bool m_IsMouseEnable;
    private bool m_IsBtnEnable = false;

    public bool getMouseState()
    {
        return m_IsMouseEnable;
    }

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
        StartCoroutine(IE_StartupTransfer());
    }

    public void gotoSelectGame()
    {
        if (!m_IsBtnEnable) return;
        StartCoroutine(IE_PlaySelectTransfer());
        GCanvasController.instance.pushToStack(m_SelectPrefab, true).GetComponent<SelectUiController>().setupManager(this);
    }

    public void loadSubGame(GSceneController.ESceneIndex index)
    {
        if (!m_IsBtnEnable) return;
        StopAllCoroutines();
        GSceneController.instance.LoadSceneAsync(index);
    }

    IEnumerator IE_StartupTransfer()
    {
        var attr = CameraAttribute.getEmpty().setPosition(Vector3.up * 50f).setRotation(Quaternion.identity).setZLength(210f).setFov(30f);
        m_Controller.setAttribute(attr);

        CameraAttribute target = CameraAttribute.getEmpty().setPosition(Vector3.up * 35f).setZLength(200f);
        yield return m_Controller.ieTransCameraCoro(target, 0.6f, t => Mathf.SmoothStep(0f, 1f, t));

        m_IsMouseEnable = true;
        m_IsBtnEnable = true;
    }

    IEnumerator IE_PlaySelectTransfer()
    {
        m_IsMouseEnable = false;

        var fov = m_Controller.getFov();
        var refFov = 0f;

        Vector3 rotator;
        Quaternion current;
        while (true)
        {
            fov = Mathf.SmoothDamp(fov, 50f, ref refFov, 0.2f);
            m_Controller.setFov(fov);

            current = m_Controller.getWorldRotation();
            rotator = current.eulerAngles;

            rotator.y -= Time.deltaTime * m_RotateSpeed;
            rotator.x = 0f;
            rotator.z = 0f;

            current = Quaternion.Slerp(current, Quaternion.Euler(rotator), 0.3f);
            m_Controller.setRotation(current);
            yield return null;
        }
    }

}
