﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiStateBar : MonoBehaviour, ICharacterDataDidChangedHandler
{
    [SerializeField]
    private Text m_CoinText;
    [SerializeField]
    private Image m_CurrentLiveImage;
    [SerializeField]
    private Text m_CurrentLiveText;
    [SerializeField]
    private Image m_HealthImage;
    [SerializeField]
    private Text m_HealthText;

    [SerializeField]
    private AnimationCurve m_TransferCurve;

    void Start()
    {
        var data = GameManager.instance.getCharacterData();
        updateUi(data);
        GameManager.instance.addEventListener(this);
    }

    void OnDestroy(){
        GameManager.instance.removeEventListener(this);
    }

    void moveToCorner()
    {
        var rect = transform as RectTransform;

        rect.pivot = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.rect.Set(80f, 50f, 320f, 200f);
    }

    void moveToCenter()
    {
        var rect = transform as RectTransform;

        rect.pivot = Vector2.one / 2f;
        rect.anchorMax = Vector2.one / 2f;
        rect.anchorMin = Vector2.one / 2f;
        rect.rect.Set(0f, 0f, 320f, 200f);
    }

    IEnumerator ieTransferStateBar()
    {
        yield return null;
    }

    void updateUi(LGameStructure.CharacterData data)
    {
        m_CoinText.text = GameManager.formatCoinValue(data.coin);

        m_CurrentLiveText.text = data.shieldPercent.ToString();
        m_CurrentLiveImage.fillAmount = data.shieldPercent;
        m_HealthText.text = data.healthPercent.ToString();
        m_HealthImage.fillAmount = data.healthPercent;
    }

    public void OnCharacterDataChanged(GameManager sender, LGameStructure.CharacterData data)
    {
        updateUi(data);
    }
}
