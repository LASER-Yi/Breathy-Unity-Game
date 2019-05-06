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

    void Update(){
        var time = GameManager.instance.getTimeOfDayFormat();
        var timeText = "";
        if(time.Key < 10){
            timeText += "0";
        }
        timeText += time.Key;
        timeText += ":";
        if (time.Value < 10)
        {
            timeText += "0";
        }
        timeText += time.Value;
        m_ClockText.text = timeText;
    }
}
