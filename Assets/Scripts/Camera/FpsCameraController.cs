using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class FpsCameraController : MonoBehaviour
{
    private CameraController m_CamController
    {
        get
        {
            return CameraController.instance;
        }
    }

    [SerializeField, Range(0f, 10f)]
    private float m_TrackingSpeed = 2f;

    [SerializeField, Range(0f, 90f)]
    private float m_HorizonalClamp = 90f;
    [SerializeField, Range(0f, 90f)]
    private float m_VerticalClamp = 60f;

    private Vector3 m_HeadRotation = Vector3.zero;

    Vector2 getUserInput(){
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }

    void updateCameraRotation(Vector2 input){
        var postInputX = input.x * m_TrackingSpeed;
        var postInputY = input.y * m_TrackingSpeed;
        m_HeadRotation += Vector3.up * postInputX + Vector3.left * postInputY;
        m_HeadRotation.y = Mathf.Clamp(m_HeadRotation.y, -m_HorizonalClamp, m_HorizonalClamp);
        m_HeadRotation.x = Mathf.Clamp(m_HeadRotation.x, -m_VerticalClamp, m_VerticalClamp);
        m_CamController.setRotation(Quaternion.Euler(m_HeadRotation));
    }

    // Update is called once per frame
    void Update()
    {
        var input = getUserInput();
        updateCameraRotation(input);
    }
}
