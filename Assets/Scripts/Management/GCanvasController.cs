using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GCanvasController : MonoBehaviour
{
    private static Object _lock = new Object();
    private static GCanvasController _instance;

    public static GCanvasController instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GCanvasController>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }
    [SerializeField]
    private RectTransform m_LoadCover;
    [SerializeField]
    private RectTransform m_EscapePanel;
    private Canvas m_Canvas;
    [SerializeField]
    private RectTransform m_StackObject;
    [SerializeField]
    private RectTransform m_CoverObject;
    private Stack<IStackableUi> m_Stack;
    // UI栈的底层，保证任何时候调用Pop都无法推出（除非清空UI）
    private int m_StackBase = 0;

    private List<ICoverableUi> m_Cover;

    void defaultEscape()
    {
        if (m_EscapePanel != null)
        {
            pushToStack(m_EscapePanel, false);
        }
    }

    void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_Stack = new Stack<IStackableUi>();
        m_Cover = new List<ICoverableUi>();
    }

    public void cleanCanvas()
    {
        m_StackBase = 0;
        cleanStack(true);
        cleanCover();
    }

    public RectTransform setupLoadCanvas()
    {
        cleanCanvas();
        return addToCover(m_LoadCover);
    }
    /* Cover */
    public RectTransform addToCover(RectTransform prefab)
    {
        if(prefab == null) return null;

        var rect = Instantiate(prefab);
        var script = rect.GetComponent<ICoverableUi>();

        if (script != null)
        {
            m_Cover.Add(script);
            rect.SetParent(m_CoverObject, false);
            script.onAddToCanvas(true);
        }
        rect.SetParent(m_CoverObject, false);
        return rect;
    }

    public void removeFromCover(RectTransform instance)
    {
        if(instance == null) return;
        
        var script = instance.GetComponent<ICoverableUi>();
        var timer = 0f;
        if (script != null)
        {
            timer = script.onRemoveFromCanvas(true);
        }
        Destroy(instance.gameObject, timer);
    }

    public void cleanCover()
    {
        foreach (var item in m_Cover)
        {
            item.onRemoveFromCanvas(false);
            Destroy(item.getTransform().gameObject);
        }
        m_Cover.Clear();
    }

    /* Stack */
    public RectTransform pushToStack(RectTransform prefab, bool isBase)
    {
        if(prefab == null) return null;
        
        var rect = Instantiate(prefab);
        var script = rect.GetComponent<IStackableUi>();

        if (script != null)
        {
            if(m_Stack.Count != 0){
                var prev = m_Stack.Peek();
                prev.onWillNotBecomeTop();
            }

            rect.SetParent(m_StackObject, false);
            m_Stack.Push(script);
            if (isBase) m_StackBase = m_Stack.Count;

            script.onDidPushToStack(true);
            script.onDidBecomeTop();
        }else{
            Destroy(rect.gameObject);
        }

        return rect;
    }

    public void cleanStack(bool ignoreBase)
    {
        var count = 0;
        if (!ignoreBase) count = m_StackBase;

        while (m_Stack.Count != count)
        {
            var script = m_Stack.Pop();
            script.onWillRemoveFromStack(false);
            Destroy(script.getTransform().gameObject);
        }
        foreach(Transform item in m_StackObject){
            Destroy(item.gameObject);
        }
    }

    public void popStack()
    {
        if (m_Stack.Count != 0)
        {
            if (m_StackBase != m_Stack.Count)
            {
                var script = m_Stack.Pop();
                var timer = script.onWillRemoveFromStack(true);
                Destroy(script.getTransform().gameObject, timer);

                if(m_Stack.Count != 0){
                    var next = m_Stack.Peek();
                    next.onDidBecomeTop();
                }
            }
            else
            {
                pushToStack(m_EscapePanel, false);
            }
        }
    }

    void checkEscapeInput()
    {
        var isPress = Input.GetButtonUp("Cancel");
        if (isPress)
        {
            popStack();
        }
    }

    void Update()
    {
        checkEscapeInput();
    }
}
