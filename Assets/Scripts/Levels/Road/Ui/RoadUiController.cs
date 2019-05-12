using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadUiController : SceneBaseUiController
{

    public override void onDidPushToStack(bool animate)
    {

    }
    public override float onWillRemoveFromStack(bool animate)
    {
        return 0f;
    }

    public float getPlayerVelocity()
    {
        if (m_SceneController is RoadSceneController rs)
        {
            return rs.getPlayerVelocity();
        }
        return 0f;
    }

    public float getFinilizeDistance()
    {
        if (m_SceneController is RoadSceneController rs)
        {
            return rs.getDistanceToEndPoint();
        }
        return 999f;
    }
}
