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

    void Awake(){
        m_UiController.pushToStack(m_Menu, true);
    }

    void Start(){
        StartCoroutine(IE_StartupTransfer());
    }

    IEnumerator IE_StartupTransfer(){
        m_Controller.setFovOnCamera(30f);
        m_Controller.setTransform(Vector3.up*35f, Quaternion.identity);

        var zlength = 210f;
        var refSpeed = 0f;
        while (zlength - 200f > float.Epsilon)
        {
            zlength = Mathf.SmoothDamp(zlength, 200f, ref refSpeed, 0.6f);
            m_Controller.setZLength(zlength);
            yield return null;
        }
    }
}
