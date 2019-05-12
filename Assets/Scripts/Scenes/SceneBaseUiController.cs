using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBaseUiController : MonoBehaviour, IStackableUi
{
    protected SceneBaseController m_SceneController{
        get{
            return SceneBaseController.instance;
        }
    }
    protected GCanvasController m_CanvasController{
        get{
            return GCanvasController.instance;
        }
    }
    protected bool m_IsEnableControl;
    public RectTransform getTransform()
    {
        return transform as RectTransform;
    }
    public abstract void onDidPushToStack(bool animate);
    public virtual void onDidBecomeTop()
    {
        m_IsEnableControl = true;
    }
    public virtual void onWillNotBecomeTop()
    {
        m_IsEnableControl = false;
    }
    public abstract float onWillRemoveFromStack(bool animate);
}
