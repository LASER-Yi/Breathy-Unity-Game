using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class UiHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text m_BtnText;

    void Awake(){
        m_BtnText = GetComponentInChildren<Text>();
    }
    public void OnPointerEnter(PointerEventData data)
    {
    }

    public void OnPointerExit(PointerEventData data)
    {
    }
    
    public void setFormatterText(string text){
        m_BtnText.text = "[ " + text + " ]";
    }

}
