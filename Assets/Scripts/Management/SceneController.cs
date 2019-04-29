using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum ESceneIndex : int
    {
        Loader = 0,
        Menu = 1,
        Road = 2,
        Market = 3,
        Heart = 4
    }
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

    void Start()
    {
        if (SceneManager.sceneCount != 2)
        {
            LoadSceneAsync(ESceneIndex.Menu);
        }
    }

    private float m_LoadProgress;

    public float getLoadProgress()
    {
        return m_LoadProgress;
    }

    public void LoadSceneAsync(ESceneIndex index)
    {
        var operation = SceneManager.LoadSceneAsync((int)index, LoadSceneMode.Additive);
        operation.priority = 0;
        operation.allowSceneActivation = false;
        StartCoroutine(IE_LoadScene(operation));
    }

    private void changeSceneSkybox(Skybox box)
    {
        if (box != null)
        {
            RenderSettings.skybox = box.material;
        }
    }

    private IEnumerator IE_LoadScene(AsyncOperation operation)
    {
        Scene? currentScene = null;
        if (SceneManager.sceneCount == 3)
        {
            currentScene = SceneManager.GetSceneAt(1);
        }
        while (operation.progress < 0.9f)
        {
            m_LoadProgress = Mathf.Lerp(0f, 0.4f, operation.progress);
            yield return null;
        }
        m_LoadProgress = 0.4f;
        // Load Complete
        if (currentScene != null)
        {
            var unloadIndex = currentScene.GetValueOrDefault().buildIndex;
            var unOperation = SceneManager.UnloadSceneAsync(unloadIndex);
            unOperation.priority = 0;
            yield return IE_UnloadScene(unOperation);
        }
        // Load New Scene
        operation.allowSceneActivation = true;
        yield return new WaitUntil(delegate () { return operation.isDone; });
        var skybox = SceneBaseController.instance.getSceneSkybox();
        changeSceneSkybox(skybox);
        yield return null;
    }

    private IEnumerator IE_UnloadScene(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            m_LoadProgress = Mathf.Lerp(0.4f, 0.9f, operation.progress);
            yield return null;
        }
        m_LoadProgress = 0.9f;
        yield return null;
    }
}
