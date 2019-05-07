using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepMainUiController : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private Text m_ClockText;
    public RectTransform getTransform()
    {
        return transform as RectTransform;
    }

    public void onDidPushToStack(bool animate)
    {

    }
    public void onDidBecomeTop()
    {

    }
    public void onWillNotBecomeTop()
    {

    }
    public float onWillRemoveFromStack(bool animate)
    {
        return 0f;
    }

    private System.Text.StringBuilder m_TextBuilder = new System.Text.StringBuilder();

    void Update(){
        var time = GameManager.instance.getTimeOfDayFormat();
        m_TextBuilder.Clear();
        if(time.Key < 10){
            m_TextBuilder.Append('0');
        }
        m_TextBuilder.Append(time.Key);
        m_TextBuilder.Append(':');
        if (time.Value < 10)
        {
            m_TextBuilder.Append('0');
        }
        m_TextBuilder.Append(time.Value);
        m_ClockText.text = m_TextBuilder.ToString();
    }
}
