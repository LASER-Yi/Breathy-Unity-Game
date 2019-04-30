using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTitleText : MonoBehaviour, ICoverableUi
{
    private Text m_TitleText;

    void Awake(){
        m_TitleText = GetComponent<Text>();
    }
    
    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onAddToCanvas(bool animate){

    }
    public float onRemoveFromCanvas(bool animate){
        return 0f;
    }
}
