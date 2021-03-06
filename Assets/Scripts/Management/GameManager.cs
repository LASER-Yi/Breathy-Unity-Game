﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameStructure;

public class GameManager : MonoBehaviour
{
    private static Object _lock = new Object();
    private static GameManager _instance;

    public static GameManager instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GameManager>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }

    public void resetWholeGame()
    {
        // 随机初始化
        m_Character.coin = Random.Range(20, 40);
        m_Character.shieldPercent = Random.Range(0.2f, 0.4f);
        m_Character.healthPercent = Random.Range(0.98f, 1f);
        fireCharacterChangedEvent();
    }

    private List<ShopItem> m_CurrentShopItem;
    private List<ShopItem> m_CurrentOwnItem;

    public List<ShopItem> getCurrentShopItems(){
        return m_CurrentShopItem;
    }

    public bool tryBuyShopItems(ShopItem item){
        if(m_Character.coin >= item.value){
            if(m_CurrentShopItem.Remove(item)){
                m_CurrentOwnItem.Add(item);
                m_Character.coin -= item.value;
                fireCharacterChangedEvent();
                return true;
            }else {
                return false;
            }
        }else{
            return false;
        }
    }

    void Awake()
    {
        resetWholeGame();
        m_CurrentShopItem = m_DataSource.getShopItemsClone();
        m_CurrentOwnItem = new List<ShopItem>();
    }

    public DataManager m_DataSource
    {
        get
        {
            return DataManager.Instance;
        }
    }

    public SleepSceneParam computeSleepParam()
    {
        var param = new SleepSceneParam();
        param.m_OwnItem = m_CurrentOwnItem;
        param.shieldRecoverRate = m_Character.healthPercent;
        return param;
    }

    public WorkSceneParam computeWorkParam()
    {
        var param = new WorkSceneParam();
        param.coinGain = Random.Range(10, 20);
        param.leaveHour = Mathf.Clamp(m_CurrentClock + 8f, 18f, 21f);
        param.timespeed = 18f;
        return param;
    }

    public RoadSceneParam computeRoadParam()
    {
        var param = new RoadSceneParam();
        param.pressForce = m_Character.shieldPercent + 0.1f;
        param.reactionDelay = 0f;
        return param;
    }

    private CharacterData m_Character;

    public CharacterData getCharacterData()
    {
        return m_Character;
    }

    private void fireCharacterChangedEvent()
    {
        characterDataChanged?.Invoke(this, m_Character);
    }

    private delegate void OnCharacterChangedHandler(GameManager sender, CharacterData data);

    private event OnCharacterChangedHandler characterDataChanged;

    public void addEventListener(ICharacterDataDidChangedHandler listener)
    {
        characterDataChanged += listener.OnCharacterDataChanged;
    }

    public void removeEventListener(ICharacterDataDidChangedHandler listener)
    {
        characterDataChanged -= listener.OnCharacterDataChanged;
    }

    public void increaseCoinValue(int val)
    {
        m_Character.coin += val;
        fireCharacterChangedEvent();
    }

    public void increaseShieldValue(float val)
    {
        m_Character.shieldPercent += val;
        m_Character.shieldPercent = Mathf.Clamp01(m_Character.shieldPercent);
        fireCharacterChangedEvent();
    }

    public void increaseHealthValue(float val)
    {
        m_Character.healthPercent += val;
        m_Character.healthPercent = Mathf.Clamp01(m_Character.healthPercent);
        fireCharacterChangedEvent();
    }

    private float m_CurrentClock;
    private int m_CurrentDayCount = 0;
    private int m_CurrentHour
    {
        get
        {
            return Mathf.FloorToInt(m_CurrentClock);
        }
    }
    private int m_CurrentMinute
    {
        get
        {
            return Mathf.FloorToInt((m_CurrentClock - m_CurrentHour) * 60f);
        }
    }

    public void increaseDayCount()
    {
        ++m_CurrentDayCount;
    }

    public int getDayCount()
    {
        return m_CurrentDayCount;
    }
    public void setClock(float original)
    {
        var willValue = original;
        if (willValue > 24) willValue -= 24f;
        m_CurrentClock = willValue;
        fireTimeChangedEvent();
    }

    public void setDeltaClock(float delta)
    {
        var current = m_CurrentClock;
        setClock(current + delta);
    }

    public void setClock(int hour, int minute)
    {
        if (hour > 24)
        {
            hour -= 24;
        }
        float time = hour + minute / 60f;

        setClock(time);
    }



    private delegate void OnGameTimeChangedHandler(GameManager sender, TimeOfGame time);

    private event OnGameTimeChangedHandler eventTimeChanged;

    public void addEventListener(ITimeDidChangedHandler listener)
    {
        eventTimeChanged += listener.OnGameTimeChanged;
    }

    public void removeEventListener(ITimeDidChangedHandler listener)
    {
        eventTimeChanged -= listener.OnGameTimeChanged;
    }

    private void fireTimeChangedEvent()
    {
        eventTimeChanged?.Invoke(this, new TimeOfGame()
        {
            hour = m_CurrentHour,
            minute = m_CurrentMinute
        });
    }

    Coroutine m_DayLoopCoro;
    public void startTimeLoop()
    {
        stopTimeLoop();
        m_DayLoopCoro = StartCoroutine(ieDayLoop());
    }

    public void stopTimeLoop()
    {
        if (m_DayLoopCoro != null)
        {
            StopCoroutine(m_DayLoopCoro);
            m_DayLoopCoro = null;
        }
    }

    public void setTimeSpeed(float newFlow)
    {
        m_MinuteOfDays = newFlow;
    }

    private float m_MinuteOfDays = 24f;

    private void checkGameOver()
    {
        if (m_Character.healthPercent <= 0f && m_Character.shieldPercent <= 0f)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    IEnumerator ieDayLoop()
    {

        while (true)
        {
            yield return new WaitForSeconds(1f);
            float deltaHour = 24f / (m_MinuteOfDays * 60f);
            var result = m_CurrentClock + deltaHour;
            setClock(result);
            // 同时流逝生命
            var lifeLoose = 1f / (m_MinuteOfDays * 30f);
            if (m_Character.shieldPercent > 0)
            {
                increaseShieldValue(-lifeLoose);
            }
            else
            {
                m_Character.shieldPercent = 0f;
                increaseHealthValue(-lifeLoose * 2f);
            }
            checkGameOver();
        }
    }

    public float startLerpTime(float toValue)
    {
        StartCoroutine(ieDayLerp(toValue, 5f));
        return 5f;
    }

    public float startLerpTime(int hour, int minute)
    {
        float value = hour;
        value += (float)minute / 60f;
        var tocSecond = value - m_CurrentClock;
        if(tocSecond < 0) tocSecond += 24f;
        StartCoroutine(ieDayLerp(value, tocSecond));
        return tocSecond;
    }

    IEnumerator ieDayLerp(float toValue, float timer)
    {
        float currentTime = 0f;
        float progress = 0f;

        float startValue = m_CurrentClock;
        float endValue = toValue;
        if (endValue < startValue) endValue += 24f;

        while (currentTime < timer)
        {
            yield return null;
            currentTime += Time.deltaTime;
            progress = currentTime / timer;

            var current = Mathf.SmoothStep(startValue, endValue, progress);
            setClock(current);
        }
    }

    /* Static */

    public static string formatCoinValue(int val){
        return "₣ " + val.ToString();
    }
}
