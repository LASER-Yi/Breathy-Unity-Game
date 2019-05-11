using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiItemPanel : MonoBehaviour
{
    private LGameStructure.ShopItem m_Item;
    [SerializeField]
    private Text m_TitleText;
    [SerializeField]
    private Text m_DescText;
    [SerializeField]
    private Text m_CoinText;
    [SerializeField]
    private Button m_ComfirmBtn;

    public void setBtnClickEvent(UiShopPanel sender){
        m_ComfirmBtn.onClick.AddListener(() => sender.buyItem(m_Item, new UnityAction(buySuccess)));
    }

    public void setupItemContent(LGameStructure.ShopItem item){
        m_TitleText.text = item.name;
        m_DescText.text = item.desc;
        m_CoinText.text = GameManager.formatCoinValue(item.value);
        m_Item = item;
    }

    void buySuccess(){
        if (SceneBaseController.instance is SleepSceneManager ss)
        {
            
            m_TitleText.text = "购买成功";
            m_DescText.text = "[1] 已经提交审核部门...\n[2]审核成功，正在等待运送...";
            m_CoinText.transform.parent.gameObject.SetActive(false);
            m_ComfirmBtn.gameObject.SetActive(false);
        }
    }
}
