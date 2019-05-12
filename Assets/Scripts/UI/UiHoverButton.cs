using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class UiHoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text m_BtnText;
    [SerializeField]
    private Image m_BtnUnderline;

    void Awake(){
        m_BtnText = GetComponentInChildren<Text>();
        m_BtnUnderline.fillAmount = 0f;
    }
    public void OnPointerEnter(PointerEventData data)
    {
        m_LerpValue = 1f;
    }

    public void OnPointerExit(PointerEventData data)
    {
        m_LerpValue = 0f;
    }

    private float m_LerpValue = 0f;

    void Update(){
        m_BtnUnderline.fillAmount = Mathf.Lerp(m_BtnUnderline.fillAmount, m_LerpValue, 0.1f);
    }
    
    public void setFormatterText(string text){
        m_BtnText.text = text;
    }

}
