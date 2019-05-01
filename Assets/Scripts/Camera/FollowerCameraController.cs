using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;


// 摄像机窗格？
public class FollowerCameraController : MonoBehaviour
{
    private CameraController m_Controller
    {
        get
        {
            return CameraController.instance;
        }
    }
    private IPawnController _chararcter;
    private IPawnController m_Character
    {
        get
        {
            if (_chararcter == null)
            {
                _chararcter = GameObject.FindGameObjectWithTag("Player").GetComponent<IPawnController>();
            }
            return _chararcter;
        }
    }
    [SerializeField]
    private float m_CameraHeight = 10f;
    [SerializeField]
    private float m_CameraFov = 90f;
    [SerializeField]
    private float m_CameraPitch = 90f;

    private Vector3 m_RefCameraSpeed;

    void initalCamera()
    {
        m_Controller.setFov(m_CameraFov);
        m_Controller.setZLength(m_CameraHeight);
        m_Controller.setPosition(m_Character.getWorldPosition());
        m_Controller.setRotation(Quaternion.Euler(m_CameraPitch, 0f, 0f));
    }

    void Start()
    {
        initalCamera();
    }

    void moveToCharacter()
    {
        var target = m_Character.getWorldPosition();
        var current = m_Controller.getWorldPosition();
        current = Vector3.SmoothDamp(current, target, ref m_RefCameraSpeed, 1f);
        m_Controller.setPosition(current);
    }

    void LateUpdate()
    {
        moveToCharacter();
    }
}
