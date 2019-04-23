using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager
{
    private static UnityEngine.Object _object;
    private static SceneManager _instance;
    // from: https://blog.csdn.net/yupu56/article/details/53668688
    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_object)
                {
                    if (_instance == null)
                    {
                        _instance = new SceneManager();
                    }
                }
            }
            return _instance;
        }
    }
    private List<Scene> m_SceneList;
}
