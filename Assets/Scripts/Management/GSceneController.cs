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
        Market = 3,
        Heart = 4
    }
    private Dictionary<ESceneIndex, string> m_SceneName;
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

    private ESceneIndex m_ActiveScene;

    public ESceneIndex getActiveScene(){
        return m_ActiveScene;
    }

    void initalSceneName()
    {
        m_SceneName = new Dictionary<ESceneIndex, string>();
        m_SceneName.Add(ESceneIndex.Road, "Road");
        m_SceneName.Add(ESceneIndex.Market, "Market");
        m_SceneName.Add(ESceneIndex.Heart, "Sleepy");
    }
    public string getSceneName(ESceneIndex index)
    {
        if (m_SceneName.ContainsKey(index))
        {
            return m_SceneName[index];
        }
        else
        {
            return null;
        }
    }

    public Dictionary<ESceneIndex, string> getSceneList()
    {
        return m_SceneName;
    }

    void Awake()
    {
        initalSceneName();
    }

    void Start()
    {
        if (SceneManager.sceneCount != 2)
        {
            LoadSceneAsync(ESceneIndex.Menu);
        }else{
            m_ActiveScene = (ESceneIndex)SceneManager.GetSceneAt(1).buildIndex;
        }
    }

    private float m_LoadProgress;

    public float getLoadProgress()
    {
        return m_LoadProgress;
    }

    public void LoadSceneAsync(ESceneIndex index)
    {
        StopAllCoroutines();
        StartCoroutine(IE_LoadScene(index));
    }

    public void setSkybox(Skybox box)
    {
        if (box != null)
        {
            RenderSettings.skybox = box.material;
        }
    }

    private IEnumerator IE_LoadScene(ESceneIndex index)
    {
        var loadScreen = GCanvasController.instance.setupLoadCanvas();
        m_LoadProgress = 0f;

        // unload prev scene
        if (SceneManager.sceneCount == 2)
        {
            var currentScene = SceneManager.GetSceneAt(1);
            if (currentScene != null) yield return IE_UnloadScene(currentScene);
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

    private IEnumerator IE_UnloadScene(Scene sce)
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
