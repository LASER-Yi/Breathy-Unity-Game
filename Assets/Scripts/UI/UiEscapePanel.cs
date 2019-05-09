using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiEscapePanel : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private Button m_ResumeButton;
    [SerializeField]
    private Button m_ExitButton;

    void Start(){
        m_ResumeButton.onClick.AddListener(clickResumeButton);
        m_ExitButton.onClick.AddListener(clickExitButton);
    }

    public void clickResumeButton(){
        GCanvasController.instance.popStack();
    }

    public void clickExitButton(){
        SceneBaseController.instance.backToMenu();
    }
    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate){

    }
    public void onDidBecomeTop(){

    }
    public void onWillNotBecomeTop(){

    }
    public float onWillRemoveFromStack(bool animate){
        return 0f;
    }
}
