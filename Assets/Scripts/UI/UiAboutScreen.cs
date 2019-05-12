using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiAboutScreen : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private Text m_AboutText;

    void Awake(){
        var info = DataManager.LoadTextSequence("Text/About");
        m_AboutText.text = info;
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
