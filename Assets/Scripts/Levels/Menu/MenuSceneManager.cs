using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneManager : SceneBaseController
{
    [SerializeField]
    private RectTransform m_SelectPrefab;

    [SerializeField]
    private float m_MouseDropDegree = 5f;
    [SerializeField, Range(0f, 1f)]
    private float m_UpDownDropPrecent = 0.5f;
    [SerializeField]
    private float m_RotateSpeed = 10f;
    private bool m_IsEnableMouse;
    private bool m_IsBtnReady = false;

    void Start()
    {
        base.Start();
        var menuUi = m_SceneUi.GetComponent<MenuUiController>();
        if (menuUi != null)
        {
            menuUi.setManager(this);
        }
        StartCoroutine(IE_StartupTransfer());
    }

    public void startSelectGame()
    {
        if (!m_IsBtnReady) return;
        StartCoroutine(IE_PlaySelectTransfer());
        GCanvasController.instance.pushToStack(m_SelectPrefab, true).GetComponent<SelectUiController>().setupManager(this);
    }

    public void transToSubGame(GSceneController.ESceneIndex index)
    {
        if (!m_IsBtnReady) return;
        GSceneController.instance.LoadSceneAsync(index);
        StopAllCoroutines();
    }

    IEnumerator IE_StartupTransfer()
    {
        m_Controller.setFovOnCamera(30f);
        m_Controller.setTransform(Quaternion.identity);

        var height = 50f;
        var refHeight = 0f;
        var zlength = 210f;
        var refLength = 0f;

        var timer = 0f;

        while (timer < 1f)
        {
            zlength = Mathf.SmoothDamp(zlength, 200f, ref refLength, 0.6f);
            height = Mathf.SmoothDamp(height, 35f, ref refHeight, 0.6f);
            m_Controller.setZLength(zlength);
            m_Controller.setTransform(Vector3.up * height, Quaternion.identity);
            yield return null;
            timer += Time.deltaTime;
        }
        m_IsEnableMouse = true;
        m_IsBtnReady = true;
    }

    IEnumerator IE_PlaySelectTransfer()
    {
        m_IsEnableMouse = false;

        var fov = m_Controller.getAttachCamera().fieldOfView;
        var refFov = 0f;

        Vector3 rotator;
        Quaternion current;
        while (true)
        {
            fov = Mathf.SmoothDamp(fov, 50f, ref refFov, 0.2f);
            m_Controller.setFovOnCamera(fov);

            current = m_Controller.getWorldRotation();
            rotator = current.eulerAngles;

            rotator.y -= Time.deltaTime * m_RotateSpeed;
            rotator.x = 0f;
            rotator.z = 0f;

            current = Quaternion.Slerp(current, Quaternion.Euler(rotator), 0.3f);
            m_Controller.setTransform(current);
            yield return null;
        }
    }

    Vector2 computeMousePercent()
    {
        if (Input.mousePresent)
        {
            var mousePos = Input.mousePosition;
            Vector2 percent = Vector2.zero;
            percent.x = mousePos.x / Screen.width;
            percent.y = mousePos.y / Screen.height;
            return percent;
        }
        else
        {
            return new Vector2(0.5f, 0.5f);
        }
    }

    void handleMouseMove()
    {
        var percent = computeMousePercent();
        var target = Vector3.zero;

        var horiDegree = m_MouseDropDegree;
        var vertDegree = horiDegree * m_UpDownDropPrecent;
        target.x = Mathf.Lerp(vertDegree, -vertDegree, percent.y);
        target.y = Mathf.Lerp(-horiDegree, horiDegree, percent.x);

        var current = m_Controller.getWorldRotation();
        current = Quaternion.Slerp(current, Quaternion.Euler(target), 0.3f);
        m_Controller.setTransform(current);
    }

    void LateUpdate()
    {
        if (m_IsEnableMouse) handleMouseMove();
    }

}
