using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GSceneController : MonoBehaviour
{
    public enum ESceneIndex : int
    {
        Loader = 0,
        Menu = 1,
        Road = 2,
        Work = 3,
        Sleep = 4
    }
    private static Object _lock = new Object();
    private static GSceneController _instance;

    public static GSceneController instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<GSceneController>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }


    private ESceneIndex m_NextScene = ESceneIndex.Menu;
    private bool m_IsLastSceneHome = false;
    private ESceneIndex m_ActiveScene;

    void Start()
    {
        if (SceneManager.sceneCount != 2)
        {
            LoadNextScene(false);
        }
        else
        {
            m_ActiveScene = (ESceneIndex)SceneManager.GetSceneAt(1).buildIndex;
        }
    }

    private float m_LoadProgress;

    public float getLoadProgress()
    {
        return m_LoadProgress;
    }

    public void updateNextScene(){
        switch (m_NextScene)
        {
            case ESceneIndex.Menu:
                {
                    m_NextScene = ESceneIndex.Sleep;
                    break;
                }
            case ESceneIndex.Sleep:
                {
                    m_NextScene = ESceneIndex.Road;
                    m_IsLastSceneHome = true;
                    break;
                }
            case ESceneIndex.Road:
                {
                    if (m_IsLastSceneHome)
                    {
                        m_NextScene = ESceneIndex.Work;
                    }
                    else
                    {
                        m_NextScene = ESceneIndex.Sleep;
                    }
                    break;
                }
            case ESceneIndex.Work:
                {
                    m_NextScene = ESceneIndex.Road;
                    m_IsLastSceneHome = false;
                    break;
                }
        }
    }

    public void LoadNextScene(bool animate)
    {
        LoadSceneAsync(m_NextScene, animate);
    }

    public void LoadSceneAsync(ESceneIndex index, bool animate)
    {
        StopAllCoroutines();
        StartCoroutine(ieLoadScene(index));
    }

    private IEnumerator ieLoadScene(ESceneIndex index)
    {
        updateNextScene();
        var loadScreen = GCanvasController.instance.setupLoadCanvas();
        m_LoadProgress = 0f;

        // unload prev scene
        if (SceneManager.sceneCount == 2)
        {
            var currentScene = SceneManager.GetSceneAt(1);
            if (currentScene != null) yield return ieUnloadScene(currentScene);
        }

        m_ActiveScene = index;
        var operation = SceneManager.LoadSceneAsync((int)index, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        // Load new scene
        while (operation.progress < 0.9f)
        {
            m_LoadProgress = Mathf.Lerp(0.4f, 0.9f, operation.progress);
            yield return null;
        }
        operation.allowSceneActivation = true;
        m_LoadProgress = 1.0f;
        yield return new WaitUntil(delegate { return operation.isDone; });
        GCanvasController.instance.removeFromCover(loadScreen);
        yield return null;
    }

    private IEnumerator ieUnloadScene(Scene sce)
    {
        var operation = SceneManager.UnloadSceneAsync(sce, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        while (!operation.isDone)
        {
            m_LoadProgress = Mathf.Lerp(0.0f, 0.5f, operation.progress);
            yield return null;
        }
        m_LoadProgress = 0.5f;
    }
}


