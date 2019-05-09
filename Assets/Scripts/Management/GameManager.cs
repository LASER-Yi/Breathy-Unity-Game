using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGameDataStruct;

namespace LGameDataStruct
{
    public struct CharacterData
    {
        public int coin;
        public float livePercent;
        public float healthPercent;
    }

    public struct WorkSceneParam
    {
        public int leaveHour;
        public float timespeed;
    }

    public struct SleepSceneParam
    {
        public float liveRecoverEffection;
    }

    public struct RoadSceneParam
    {
        public float reactionTime;
    }

    public struct ShopItem
    {
        public int value;
        public string title;
        public string desc;
        public string prefabPath;       //In Resource
    }
}

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

    public void resetWholeGame(){
        // 随机初始化
        m_Character.coin = Random.Range(20, 40);
        m_Character.livePercent = Random.Range(0.2f, 0.4f);
        m_Character.healthPercent = Random.Range(0.9f, 1f);
    }

    // public DataManager m_SaveDataManager
    // {
    //     get
    //     {
    //         return DataManager.Instance;
    //     }
    // }

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


    public void addCoinValue(int val)
    {
        m_Character.coin += val;
    }


    private int m_CurrentDayCount = 0;
    private int m_CurrentHour
    {
        get
        {
            return Mathf.FloorToInt(m_CurrentTimeOfDay);
        }
    }
    private int m_CurrentMinute
    {
        get
        {
            return Mathf.FloorToInt((m_CurrentTimeOfDay - m_CurrentHour) * 60f);
        }
    }

    private float m_CurrentTimeOfDay;


    public void addDayCount()
    {
        ++m_CurrentDayCount;
    }

    public int getDayCount()
    {
        return m_CurrentDayCount;
    }
    public void setTimeOfDay(float original)
    {
        var willValue = original;
        if (willValue > 24) willValue -= 24f;
        m_CurrentTimeOfDay = willValue;
        fireTimeChangedEvent();
    }

    public void setTimeDelta(float delta)
    {
        var current = m_CurrentTimeOfDay;
        setTimeOfDay(current + delta);
    }

    public void setTimeOfDay(int hour, int minute)
    {
        if (hour > 24)
        {
            hour -= 24;
        }
        float time = hour + minute / 60f;

        setTimeOfDay(time);
    }

    public struct TimeOfGame
    {
        public int hour;
        public int minute;
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

    IEnumerator ieDayLoop()
    {

        while (true)
        {
            float deltaHour = 24f / (m_MinuteOfDays * 60f);
            var result = m_CurrentTimeOfDay + deltaHour;
            setTimeOfDay(result);
            yield return new WaitForSeconds(1f);
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
        StartCoroutine(ieDayLerp(value, 5f));
        return 5f;
    }

    IEnumerator ieDayLerp(float toValue, float timer)
    {
        float currentTime = 0f;
        float progress = 0f;
        float endValue = toValue;
        if (endValue < toValue) endValue += 24;

        float startValue = m_CurrentTimeOfDay;

        while (currentTime < timer)
        {
            yield return null;
            currentTime += Time.deltaTime;
            progress = currentTime / timer;

            progress = Mathf.Pow(progress, 2f);

            var current = Mathf.SmoothStep(startValue, endValue, progress);
            setTimeOfDay(current);
        }
    }
}

public interface ICharacterDataDidChangedHandler
{
    void OnCharacterDataChanged(GameManager sender, CharacterData data);
}

public interface ITimeDidChangedHandler
{
    void OnGameTimeChanged(GameManager sender, GameManager.TimeOfGame time);
}
