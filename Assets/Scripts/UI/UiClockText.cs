using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiClockText : MonoBehaviour, ITimeDidChangedHandler
{
    [SerializeField]
    private Text m_ClockText;
    private System.Text.StringBuilder m_TextBuilder;
    void Awake(){
        m_TextBuilder = new System.Text.StringBuilder();
        GameManager.instance.addEventListener(this);
    }

    void OnDestroy(){
        GameManager.instance.removeEventListener(this);
    }

    public void OnGameTimeChanged(GameManager sender, LGameStructure.TimeOfGame time)
    {
        m_TextBuilder.Clear();
        if (time.hour < 10)
        {
            m_TextBuilder.Append('0');
        }
        m_TextBuilder.Append(time.hour);
        m_TextBuilder.Append(':');
        if (time.minute < 10)
        {
            m_TextBuilder.Append('0');
        }
        m_TextBuilder.Append(time.minute);
        m_ClockText.text = m_TextBuilder.ToString();
    }
}
