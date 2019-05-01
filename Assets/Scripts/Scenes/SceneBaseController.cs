using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneBaseController : MonoBehaviour
{
    private static Object _lock = new Object();
    private static SceneBaseController _instance;

    public static SceneBaseController instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var obj = FindObjectOfType<SceneBaseController>();
                    if (obj != null)
                    {
                        _instance = obj;
                    }
                }
            }
            return _instance;
        }
    }

    protected GCanvasController m_UiController
    {
        get
        {
            return GCanvasController.instance;
        }
    }

    [SerializeField]
    private bool m_IsShowAfterLoad = false;
    protected string m_SceneName;
    [SerializeField]
    private RectTransform m_TitlePrefab;

    [SerializeField]
    private Skybox m_SceneSkybox;

    [SerializeField]
    private RectTransform m_SceneUiPrefab;
    protected RectTransform m_SceneUi;

    public void backToMenu()
    {
        GSceneController.instance.LoadSceneAsync(GSceneController.ESceneIndex.Menu);
    }

    private void initalSceneInfo(){
        var index = GSceneController.instance.getActiveScene();
        m_SceneName = GSceneController.instance.getSceneName(index);
        GSceneController.instance.setSkybox(m_SceneSkybox);
    }

    protected void Start()
    {
        initalSceneInfo();
        m_SceneUi = m_UiController.pushToStack(m_SceneUiPrefab, true);
        if (m_IsShowAfterLoad)
        {
            StartCoroutine(ieShowTitle());
        }
    }

    IEnumerator ieShowTitle()
    {
        var title = GCanvasController.instance.addToCover(m_TitlePrefab);
        var text = title.GetComponent<UnityEngine.UI.Text>();
        if (text != null) text.text = m_SceneName;
        yield return new WaitForSecondsRealtime(3.0f);
        GCanvasController.instance.removeFromCover(title);
    }

    protected looper updatePreLoop;
    protected checker checkPreState;
    protected looper updateMainLoop;
    protected checker checkMainState;
    protected looper updateEndLoop;
    protected checker checkEndState;

    public void startGameLoop(){
        StartCoroutine(ieGameProcess());
    }

    protected delegate bool checker();

    protected delegate void looper();

    IEnumerator ieGameProcess(){
        if(checkPreState != null && updatePreLoop != null){
            yield return ieCheckAndLoop(checkPreState, updatePreLoop);
        }
        if(checkMainState != null && updateMainLoop != null){
            yield return ieCheckAndLoop(checkMainState, updateMainLoop);
        }
        if(checkEndState != null && updateEndLoop != null){
            yield return ieCheckAndLoop(checkEndState, updateEndLoop);
        }
        yield return null;
        backToMenu();
    }

    IEnumerator ieCheckAndLoop(checker chk, looper lop){
        while (chk())
        {
            lop();
            yield return null;
        }
    }
}
