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

    void initalSceneName(){
        m_SceneName = new Dictionary<ESceneIndex, string>();
        m_SceneName.Add(ESceneIndex.Road, "Road");
        m_SceneName.Add(ESceneIndex.Market, "Market");
        m_SceneName.Add(ESceneIndex.Heart, "Sleepy");
    }
    public string getSceneName(ESceneIndex index){
        if(m_SceneName.ContainsKey(index)){
            return m_SceneName[index];
        }else{
            return null;
        }
    }

    public Dictionary<ESceneIndex, string> getSceneList(){
        return m_SceneName;
    }

    void Awake(){
        initalSceneName();
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
        StartCoroutine(IE_LoadScene(operation, index));
    }

    private void changeSceneSkybox(Skybox box)
    {
        if (box != null)
        {
            RenderSettings.skybox = box.material;
        }
    }

    private IEnumerator IE_LoadScene(AsyncOperation operation, ESceneIndex index)
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
        var name = getSceneName(index);
        if(name != null){
            SceneBaseController.instance.setSceneName(name);
        }
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
