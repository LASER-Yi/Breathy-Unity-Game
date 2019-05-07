using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepMainUiController : MonoBehaviour, IStackableUi
{
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
    }
}
