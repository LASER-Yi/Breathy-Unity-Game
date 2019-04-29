using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUiController : MonoBehaviour
{

    private MenuSceneManager m_Manager;
    private HorizontalLayoutGroup m_BtnContainer;

    [SerializeField]
    private RectTransform m_BtnPrefab;

    void Awake(){
        m_BtnContainer = GetComponentInChildren<HorizontalLayoutGroup>();
        
        if(m_BtnPrefab != null){
            {
                var _object = Instantiate(m_BtnPrefab);
                var text = _object.GetComponentInChildren<Text>();
                text.text = "开始游戏";
                var btn = _object.GetComponent<Button>();
                btn.onClick.AddListener(onStartPress);
                _object.SetParent(m_BtnContainer.transform, false);
            }
            {
                var _object = Instantiate(m_BtnPrefab);
                var text = _object.GetComponentInChildren<Text>();
                text.text = "游戏设置";
                var btn = _object.GetComponent<Button>();
                btn.onClick.AddListener(onSettingPress);
                _object.SetParent(m_BtnContainer.transform, false);
            }

            {
                var _object = Instantiate(m_BtnPrefab);
                var text = _object.GetComponentInChildren<Text>();
                text.text = "关于游戏";
                var btn = _object.GetComponent<Button>();
                btn.onClick.AddListener(onInfoPress);
                _object.SetParent(m_BtnContainer.transform, false);
            }

            {
                var _object = Instantiate(m_BtnPrefab);
                var text = _object.GetComponentInChildren<Text>();
                text.text = "结束游戏";
                var btn = _object.GetComponent<Button>();
                btn.onClick.AddListener(onExitPress);
                _object.SetParent(m_BtnContainer.transform, false);
            }
        }

    }

    public void setManager(MenuSceneManager instance)
    {
        m_Manager = instance;
    }
    public void onStartPress()
    {
        m_Manager.startSelectGame();
    }

    public void onSettingPress()
    {

    }

    public void onInfoPress()
    {

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
