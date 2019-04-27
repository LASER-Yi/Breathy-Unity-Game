using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamManager : MonoBehaviour
{
    private CameraController m_Controller{
        get{
            return CameraController.instance;
        }
    }
}
