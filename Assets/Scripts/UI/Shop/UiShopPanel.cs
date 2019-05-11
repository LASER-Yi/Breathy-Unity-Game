using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameStructure;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiShopPanel : MonoBehaviour, IStackableUi, ICharacterDataDidChangedHandler
{
    [SerializeField]
    private RectTransform m_ItemPrefab;
    [SerializeField]
    private RectTransform m_ScrollViewContent;
    [SerializeField]
    private Text m_CoinValueText;
    [SerializeField]
    private Text m_AlarmText;
    private UiBlurPanel m_BlurPanel;

    void Awake(){
        m_BlurPanel = GetComponentInChildren<UiBlurPanel>();
        m_CoinValueText.text = GameManager.formatCoinValue(GameManager.instance.getCharacterData().coin);
        GameManager.instance.addEventListener(this);
        m_AlarmText.gameObject.SetActive(false);
    }

    void OnDestroy(){
        GameManager.instance.removeEventListener(this);
    }

    public void OnCharacterDataChanged(GameManager sender, CharacterData data){
        m_CoinValueText.text = GameManager.formatCoinValue(data.coin);
    }

    public void showItem(List<ShopItem> items, int coin){
        foreach(var item in items){
            setupShopItem(item);
        }
    }

    private void setAlarm(string text, float second){
        m_AlarmText.text = text;
        m_AlarmText.gameObject.SetActive(true);
    }

    public void buyItem(ShopItem item, UnityAction sucCallback){
        if(GameManager.instance.tryBuyShopItems(item)){
            sucCallback.Invoke();
        }else{
            setAlarm("₣币不足", 2f);
        }
    }

    void setupShopItem(ShopItem item){
        var _obj = Instantiate(m_ItemPrefab);
        var script = _obj.GetComponent<UiItemPanel>();
        script.setupItemContent(item);
        script.setBtnClickEvent(this);
        _obj.SetParent(m_ScrollViewContent, false);
    }

    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate){
        m_BlurPanel.setTransparent(1f);
        m_BlurPanel.setTransparent(0f, 0.3f);
    }
    public void onDidBecomeTop(){

    }
    public void onWillNotBecomeTop(){

    }
    public float onWillRemoveFromStack(bool animate){
        m_BlurPanel.setTransparent(1f, 0.3f);
        return 0.3f;
    }
}
