using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketPlayerPawn : MonoBehaviour
{
    private IGamePawnBaseController m_Controller;
    void Awake(){
        m_Controller = GetComponent<IGamePawnBaseController>();
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
