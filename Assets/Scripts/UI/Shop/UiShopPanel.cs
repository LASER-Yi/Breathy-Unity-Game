using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameStructure;

public class UiShopPanel : MonoBehaviour, IStackableUi
{
    [SerializeField]
    private RectTransform m_ItemPrefab;
    // Start is called before the first frame update
    private RectTransform m_ScrollViewContent;

    private UiBlurPanel m_BlurPanel;

    void Awake(){
        m_BlurPanel = GetComponentInChildren<UiBlurPanel>();
    }

    public void showItem(List<ShopItem> items, int coin){

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
        return 0.5f;
    }
}
