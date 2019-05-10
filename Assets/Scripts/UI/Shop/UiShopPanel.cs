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

    public void showItem(List<ShopItem> items, int coin){

    }

    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate){

    }
    public void onDidBecomeTop(){

    }
    public void onWillNotBecomeTop(){

    }
    public float onWillRemoveFromStack(bool animate){
        return 0f;
    }
}
