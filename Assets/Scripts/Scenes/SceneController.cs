﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneController : MonoBehaviour
{
    [SerializeField]
    private string m_SceneName;

    public string getSceneName(){
        return m_SceneName;
    }
}
