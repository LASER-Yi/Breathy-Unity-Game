using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 摄像机窗格？
public class MarketCameraController : MonoBehaviour
{
    private CameraController m_Controller
    {
        get
        {
            return CameraController.instance;
        }
    }
    private IGamePawnBaseController _chararcter;
    private IGamePawnBaseController m_Character
    {
        get
        {
            if (_chararcter == null)
            {
                _chararcter = GameObject.FindGameObjectWithTag("Player").GetComponent<IGamePawnBaseController>();
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
        m_Controller.setFovOnCamera(m_CameraFov);
        m_Controller.setZLength(m_CameraHeight);
        m_Controller.setTransform(m_Character.getWorldPosition());
        m_Controller.setTransform(Quaternion.Euler(m_CameraPitch, 0f, 0f));
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
        m_Controller.setTransform(current);
    }

    void LateUpdate()
    {
        moveToCharacter();
    }
}
