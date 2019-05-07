using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadUiController : MonoBehaviour, IStackableUi
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
}
