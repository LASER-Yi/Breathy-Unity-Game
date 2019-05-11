using System.Collections;
using System.Collections.Generic;

namespace LGameStructure
{
    public struct CharacterData
    {
        public int coin;
        public float shieldPercent;
        public float healthPercent;
    }

    public struct WorkSceneParam
    {
        public float leaveHour;
        public float timespeed;
        public int coinGain;
    }

    public struct SleepSceneParam
    {
        public List<ShopItem> m_OwnItem;
        public float shieldRecoverRate;
    }

    public struct RoadSceneParam
    {
        public float pressForce;
        public float reactionDelay;
    }

    [System.Serializable]
    public struct ShopItem
    {
        public string name;
        public string desc;
        public int value;
        public string prefabPath;       //In Resource
    }

    [System.Serializable]
    public struct SaveData
    {
        public int dayCount;
        public GSceneController.ESceneIndex sceneIndex;
    }

    public struct TimeOfGame
    {
        public int hour;
        public int minute;
    }
}
