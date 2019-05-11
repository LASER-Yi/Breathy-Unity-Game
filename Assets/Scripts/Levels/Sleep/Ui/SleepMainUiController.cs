using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepMainUiController : SceneBaseUiController
{
    private UiActionContainer m_ActionContainer;

    [SerializeField]
    private RectTransform m_ShopPanelPrefab;

    void Awake(){
        m_ActionContainer = GetComponentInChildren<UiActionContainer>();
    }

    public void showStartupAction(SleepSceneManager sender)
    {
        m_ActionContainer.cleanAllAction();
        m_ActionContainer.setupAction(KeyCode.Q, sender.clickShopBtn, "Q", "商店");
        m_ActionContainer.setupAction(KeyCode.Space, sender.clickSleepBtn, "SPACE", "睡觉");
    }

    public void showWaitAction()
    {
        m_ActionContainer.cleanAllAction();
        m_ActionContainer.setupAction(null, null, "......", "等待");
    }

    public void showWakeupAction(SleepSceneManager sender)
    {
        m_ActionContainer.cleanAllAction();
        m_ActionContainer.setupAction(KeyCode.Space, sender.loadNextScene, "SPACE", "上班");
    }

    public void setupShopPanel(List<LGameStructure.ShopItem> items, int coin){
        var instance = GCanvasController.instance.pushToStack(m_ShopPanelPrefab, false).GetComponent<UiShopPanel>();
        instance.showItem(items, coin);
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

    public override void onDidPushToStack(bool animate)
    {

    }
    public override void onDidBecomeTop()
    {
        base.onDidBecomeTop();
        m_ActionContainer.enableAction();
    }
    public override void onWillNotBecomeTop()
    {
        base.onWillNotBecomeTop();
        m_ActionContainer.disableAction();
    }
    public override float onWillRemoveFromStack(bool animate)
    {
        return 0f;
    }
}
