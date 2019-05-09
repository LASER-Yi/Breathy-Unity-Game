﻿using System.Collections;
using System.Collections.Generic;

namespace LGameStructure
{
    public struct CharacterData
    {
        public int coin;
        public float livePercent;
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
        public float liveRecoverEffection;
    }

    public struct RoadSceneParam
    {
        public float reactionDelay;
    }

    public struct ShopItem
    {
        public int value;
        public string title;
        public string desc;
        public string prefabPath;       //In Resource
    }

    public struct TimeOfGame
    {
        public int hour;
        public int minute;
    }
}
