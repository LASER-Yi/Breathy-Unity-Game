using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LCameraSystem;

public class MenuUiController : SceneBaseUiController
{
    private VerticalLayoutGroup m_BtnContainer;
    [SerializeField]
    private Text m_AnyKeyText;

    [SerializeField]
    private RectTransform m_BtnPrefab;

    [SerializeField]
    private RectTransform m_AboutPrefab;

    void Start(){
        m_BtnContainer.gameObject.SetActive(false);
    }

    public void showAboutScreen(){
        m_CanvasController.pushToStack(m_AboutPrefab, false);
    }

    public void cleanBtnContainer(){
        foreach(Transform item in m_BtnContainer.transform){
            Destroy(item.gameObject);
        }
    }

    public void setupButton(string name, UnityAction action){
        var _object = Instantiate(m_BtnPrefab);
        var btnc = _object.GetComponent<UiHoverButton>();
        btnc.setFormatterText(name);
        var btn = _object.GetComponent<Button>();
        btn.onClick.AddListener(action);
        _object.SetParent(m_BtnContainer.transform, false);
    }

    void Awake()
    {
        m_BtnContainer = GetComponentInChildren<VerticalLayoutGroup>();
    }

    public void hideAnyText(){
        m_AnyKeyText.gameObject.SetActive(false);
        m_BtnContainer.gameObject.SetActive(true);
    }
    public override void onDidPushToStack(bool animate){

    }
    public override float onWillRemoveFromStack(bool animate){
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
