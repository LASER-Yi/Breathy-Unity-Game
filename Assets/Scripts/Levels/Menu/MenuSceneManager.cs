using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneManager : SceneBaseController
{
    [SerializeField]
    private RectTransform m_SelectPrefab;

    private float m_RotateSpeed = 10f;
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

        m_CamController.setAttribute(attr);

        CameraAttribute target = CameraAttribute.Empty;
        target.setPosition(Vector3.up * 35f);
        target.setZLength(200f);

        yield return m_CamController.ieTransCameraCoro(target, 0.6f, t => Mathf.SmoothStep(0f, 1f, t));

        m_IsMouseEnable = true;
        m_IsBtnEnable = true;
    }

    IEnumerator IE_PlaySelectTransfer()
    {
        m_IsMouseEnable = false;

        var fov = m_CamController.getFov();
        var refFov = 0f;

        Vector3 rotator;
        Quaternion current;
        while (true)
        {
            fov = Mathf.SmoothDamp(fov, 50f, ref refFov, 0.2f);
            m_CamController.setFov(fov);

            current = m_CamController.getWorldRotation();
            rotator = current.eulerAngles;

            rotator.y -= Time.deltaTime * m_RotateSpeed;
            rotator.x = 0f;
            rotator.z = 0f;

            current = Quaternion.Slerp(current, Quaternion.Euler(rotator), 0.3f);
            m_CamController.setRotation(current);
            yield return null;
        }
    }

}
