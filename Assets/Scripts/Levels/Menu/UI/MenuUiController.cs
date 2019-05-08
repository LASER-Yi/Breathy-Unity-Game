using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LCameraSystem;

public class MenuUiController : MonoBehaviour, IStackableUi
{
    private MenuSceneManager m_Manager;
    private HorizontalLayoutGroup m_BtnContainer;

    [SerializeField]
    private RectTransform m_BtnPrefab;

    // [SerializeField]
    // private float m_MouseDropDegree = 5f;
    // [SerializeField, Range(0f, 1f)]
    // private float m_UpDownDropPrecent = 0.5f;

    void Awake()
    {
        m_BtnContainer = GetComponentInChildren<HorizontalLayoutGroup>();

        if (m_BtnPrefab != null)
        {
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

    // TODO: 修改为MVC模式
    public void setManager(MenuSceneManager instance)
    {
        m_Manager = instance;
    }
    public void onStartPress()
    {
        
        m_Manager.startGame();
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

    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate){

    }
    public void onDidBecomeTop(){
        gameObject.SetActive(true);
    }
    public void onWillNotBecomeTop(){
        gameObject.SetActive(false);
    }
    public float onWillRemoveFromStack(bool animate){
        return 0f;
    }

    // Vector2 computeMousePercent()
    // {
    //     if (Input.mousePresent)
    //     {
    //         var mousePos = Input.mousePosition;
    //         Vector2 percent = Vector2.zero;
    //         percent.x = mousePos.x / Screen.width;
    //         percent.y = mousePos.y / Screen.height;
    //         return percent;
    //     }
    //     else
    //     {
    //         return new Vector2(0.5f, 0.5f);
    //     }
    // }

    // void handleMouseMove()
    // {
    //     var percent = computeMousePercent();
    //     var target = Vector3.zero;

    //     var horiDegree = m_MouseDropDegree;
    //     var vertDegree = horiDegree * m_UpDownDropPrecent;
    //     target.x = Mathf.Lerp(vertDegree, -vertDegree, percent.y);
    //     target.y = Mathf.Lerp(-horiDegree, horiDegree, percent.x);

    //     var current = CameraController.instance.getWorldRotation();
    //     current = Quaternion.Slerp(current, Quaternion.Euler(target), 0.3f);
    //     CameraController.instance.setRotation(current);
    // }

    // void LateUpdate()
    // {
    //     handleMouseMove();
    // }

}
