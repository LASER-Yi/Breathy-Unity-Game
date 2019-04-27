using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUiController : MonoBehaviour
{

    private MenuManager m_Manager;

    public void setManager(MenuManager instance){
        m_Manager = instance;
    }
    public void onStartPress()
    {
        m_Manager.startSelectGame();
    }

    public void onExitPress()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void onBtnHover()
    {

    }
}
