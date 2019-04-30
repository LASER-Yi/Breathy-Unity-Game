using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPlayerPawn : MonoBehaviour
{
    private IPawnController m_Controller;
    void Awake(){
        m_Controller = GetComponent<IPawnController>();
    }

    void handleUserInput(){
        var btnPress = Input.GetButtonUp("Jump");
        if(btnPress){
            m_Controller.updateUserInput(Vector3.forward);
        }
    }
    void Update()
    {
        handleUserInput();
    }
}
