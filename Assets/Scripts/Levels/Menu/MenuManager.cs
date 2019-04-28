using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : SceneBaseController
{
    private CameraController m_Controller{
        get{
            return CameraController.instance;
        }
    }

    private UiCanvasController m_UiController{
        get{
            return UiCanvasController.instance;
        }
    }
    [SerializeField]
    private RectTransform m_Menu;
    [SerializeField]
    private float m_MouseDropDegree = 5f;
    [SerializeField, Range(0f, 1f)]
    private float m_UpDownDropPrecent = 0.5f;
    private bool m_IsEnableMouse;

    void Awake(){
        var rect = m_UiController.pushToStack(m_Menu, true);
        var menu = rect.GetComponent<MenuUiController>();
        if(menu != null){
            menu.setManager(this);
        }
    }

    void Start(){
        StartCoroutine(IE_StartupTransfer());
    }

    public void startSelectGame(){
        StartCoroutine(IE_PlaySelectTransfer());
    }

    IEnumerator IE_StartupTransfer(){
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
    }

    IEnumerator IE_PlaySelectTransfer(){
        m_IsEnableMouse = false;

        var fov = m_Controller.getAttachCamera().fieldOfView;
        var refFov = 0f;
        var zlength = m_Controller.getZLength();
        var refZlength = 0f;

        Vector3 rotator;
        Quaternion current;
        while (true)
        {
            fov = Mathf.SmoothDamp(fov, 50f, ref refFov, 0.2f);
            m_Controller.setFovOnCamera(fov);

            // zlength = Mathf.SmoothDamp(zlength, 190f, ref refZlength, 0.6f);
            // m_Controller.setZLength(zlength);

            current = m_Controller.getWorldRotation();
            rotator = current.eulerAngles;

            rotator.y -= Time.deltaTime * 10f;
            rotator.x = 0f;
            rotator.z = 0f;

            current = Quaternion.Lerp(current, Quaternion.Euler(rotator), 0.3f);
            m_Controller.setTransform(current);
            yield return null;
        }
    }

    Vector2 computeMousePercent(){
        if (Input.mousePresent)
        {
            var mousePos = Input.mousePosition;
            Vector2 percent = Vector2.zero;
            percent.x = mousePos.x / Screen.width;
            percent.y = mousePos.y / Screen.height;
            return percent;
        }else{
            return new Vector2(0.5f, 0.5f);
        }
    }

    void handleMouseMove(){
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

    void LateUpdate(){
        if(m_IsEnableMouse) handleMouseMove();
    }
}
