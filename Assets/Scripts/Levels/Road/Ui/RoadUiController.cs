using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadUiController : MonoBehaviour, IStackableUi
{
    private RoadSceneManager m_Controller;

    void Start()
    {
        if (SceneBaseController.instance is RoadSceneManager cs)
        {
            m_Controller = cs;
        }
    }
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

    public float getPlayerVelocity()
    {
        return m_Controller.getPlayerVelocity();
    }

    public float getFinilizeDistance()
    {
        return m_Controller.getDistanceToEndPoint();
    }
}
