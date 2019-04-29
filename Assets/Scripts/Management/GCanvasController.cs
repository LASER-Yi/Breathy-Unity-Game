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
    private RectTransform m_LoadPrefab;

    private RectTransform m_MenuPrefab;
    private Canvas m_Canvas;
    private Stack<RectTransform> m_Stack;
    // UI栈的底层，保证任何时候调用Pop都无法推出
    private int m_StackBase = 0;

    void Awake(){
        m_Canvas = GetComponent<Canvas>();
        m_Stack = new Stack<RectTransform>();
    }

    public RectTransform putToCanvas(RectTransform prefab){
        var _obj = Instantiate(prefab);
        _obj.SetParent(m_Canvas.transform, false);
        return _obj;
    }

    void cleanupCanvas(){
        m_StackBase = 0;
        foreach (var item in m_Stack)
        {
            Destroy(item.gameObject);
        }
        m_Stack.Clear();
    }

    void setupLoadCanvas(){
        cleanupCanvas();
    }

    public RectTransform pushToStack(RectTransform prefab, bool isBase){
        var rect = Instantiate(prefab);
        rect.SetParent(m_Canvas.transform, false);
        m_Stack.Push(rect);

        if(isBase) m_StackBase = m_Stack.Count;

        return rect;
    }

    // 推出UI栈直到发现这个prefab为止
    public void popStack(RectTransform prefab){
        while (m_Stack.Count != 0)
        {
            var peek = m_Stack.Peek();
            if(peek == prefab){
                break;
            }else {
                m_Stack.Pop();
            }
        }
    }

    public void popFromStack(){
        if(m_Stack.Count != 0){
            if (m_StackBase != m_Stack.Count)
            {
                var rect = m_Stack.Pop();
                Destroy(rect);
            }
            else
            {
                if (m_MenuPrefab != null)
                {
                    pushToStack(m_MenuPrefab, false);
                }
            }
        }
    }

    public void setMenuPrefab(RectTransform prefab){
        m_MenuPrefab = prefab;
    }

    void handleEscapeEvent(){
        var isPress = Input.GetButtonUp("Cancel");
        if(isPress){
            popFromStack();
        }
    }

    void update(){
        handleEscapeEvent();
    }
}
