using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTitleText : MonoBehaviour, ICoverableUi
{
    private Text m_TitleText;

    [SerializeField] 
    private Text m_DayCountText;

    void Awake(){
        m_TitleText = GetComponent<Text>();
        var days = GameManager.instance.getDayCount();
        if(m_TitleText.text != ""){
            m_DayCountText.text = "Day " + days;
        }else{
            m_DayCountText.text = "";
        }
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
