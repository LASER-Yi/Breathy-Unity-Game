using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepMainUiController : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private RectTransform m_ActionContainer;
    [SerializeField]
    private RectTransform m_ActionLabel;

    void Awake()
    {
        m_ActionDic = new Dictionary<KeyCode, actionFunction>();
        m_ActionList = new List<UiActionLabel>();
    }

    private delegate void actionFunction();

    private Dictionary<KeyCode, actionFunction> m_ActionDic;

    private List<UiActionLabel> m_ActionList;

    public void cleanAllAction()
    {
        foreach (var item in m_ActionList)
        {
            Destroy(item.gameObject);
        }
        m_ActionList.Clear();
        m_ActionDic.Clear();
    }

    void setupAction(KeyCode? code, actionFunction func, string name, string actionName)
    {
        var _obj = Instantiate(m_ActionLabel);
        var script = _obj.GetComponent<UiActionLabel>();
        script.setActionLabel(name, actionName);
        _obj.SetParent(m_ActionContainer, false);
        m_ActionList.Add(script);
        if (code is KeyCode kc) m_ActionDic.Add(kc, func);
    }

    public void showStartupAction(SleepSceneManager sender)
    {
        showWaitAction();
        setupAction(KeyCode.Space, sender.prepareSleep, "SPACE", "睡觉");
    }

    public void showWaitAction()
    {
        cleanAllAction();
        setupAction(null, null, "......", "等待");
    }

    public void showWakeupAction(SleepSceneManager sender)
    {
        setupAction(KeyCode.Space, sender.loadNextScene, "SPACE", "上班");
    }

    public IEnumerator transformStateBar()
    {
        var timer = 3f;
        var currentTime = 0f;
        while (currentTime < timer)
        {
            yield return null;
            currentTime += Time.deltaTime;

        }
    }

    void checkUserInput()
    {
        foreach (var item in m_ActionDic)
        {
            if (Input.GetKeyUp(item.Key))
            {
                item.Value();
                break;
            }
        }
    }

    void Update()
    {
        checkUserInput();
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
}
