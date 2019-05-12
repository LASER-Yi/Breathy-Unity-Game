using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 车辆行为
// 左右转只能在前进时候进行
// 松开油门会减速

[RequireComponent(typeof(CarController))]
public class CarPlayerPawn : MonoBehaviour
{
    private IPawnController m_Controller;
    private float m_IncreaseRate = 0.1f;

    void Awake()
    {
        m_Controller = GetComponent<IPawnController>();
    }

    void Start(){
        var manager = SceneBaseController.instance as RoadSceneController;
        m_IncreaseRate = manager.getPressForce();
    }

    void Update()
    {
        var input = handleUserInput();
        if (m_Controller != null) m_Controller.updateUserInput(input);
    }

    private float m_CurrentPower = 0.5f;

    public Vector3 handleUserInput()
    {
        float horizon = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool speedup = Input.GetButtonDown("Jump");
        if (speedup)
        {
            m_CurrentPower += m_IncreaseRate;
        }
        else
        {
            m_CurrentPower -= Time.deltaTime / 2f;
        }
        m_CurrentPower = Mathf.Clamp01(m_CurrentPower);

        Vector3 input = Vector3.zero;
        input += Vector3.forward * m_CurrentPower;
        input += Vector3.down * vertical;
        input += Vector3.right * horizon;
        return input;
    }
}