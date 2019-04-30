using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSceneManager : SceneBaseController
{

    void setupCamera(){
        m_Controller.setTransform(Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
        m_Controller.setZLength(20f);
        m_Controller.setFovOnCamera(90f);
    }
    void Start()
    {
        base.Start();
        setupCamera();
    }
}
