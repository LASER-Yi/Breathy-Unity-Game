using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiActionContainer : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_ActionLabel;
    private Dictionary<KeyCode, UnityAction> m_ActionDic;
    private List<UiActionLabel> m_ActionList;
    private bool m_IsActionEnable = false;

    void Awake(){
        m_ActionList = new List<UiActionLabel>();
        m_ActionDic = new Dictionary<KeyCode, UnityAction>();
    }

    public void enableAction(){
        m_IsActionEnable = true;
    }

    public void disableAction(){
        m_IsActionEnable = false;
    }

    public void cleanAllAction()
    {
        foreach (var item in m_ActionList)
        {
            Destroy(item.gameObject);
        }
        m_ActionList.Clear();
        m_ActionDic.Clear();
    }

    public void setupAction(KeyCode? code, UnityAction action, string name, string actionName)
    {
        var _obj = Instantiate(m_ActionLabel);
        var script = _obj.GetComponent<UiActionLabel>();
        script.setActionLabel(name, actionName);
        _obj.SetParent(transform, false);
        m_ActionList.Add(script);
        if (code is KeyCode kc) m_ActionDic.Add(kc, action);
    }

    void checkInput(){
        foreach (var item in m_ActionDic)
        {
            if (Input.GetKeyUp(item.Key))
            {
                item.Value.Invoke();
                break;
            }
        }
    }

    void Update(){
        if(m_IsActionEnable) checkInput();
    }
}
