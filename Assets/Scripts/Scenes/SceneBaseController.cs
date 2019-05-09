using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

    protected GameManager m_Game{
        get{
            return GameManager.instance;
        }
    }

    protected GCanvasController m_UiController
    {
        get
        {
            return GCanvasController.instance;
        }
    }

    protected LCameraSystem.CameraController m_CamController
    {
        get
        {
            return LCameraSystem.CameraController.instance;
        }
    }

    [SerializeField]
    private bool m_IsShowAfterLoad = false;
    [SerializeField]
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
        GSceneController.instance.LoadSceneAsync(GSceneController.ESceneIndex.Menu, false);
    }

    public void attachSceneUi()
    {
        m_UiController.cleanStack(true);
        m_SceneUi = m_UiController.pushToStack(m_SceneUiPrefab, true);
    }

    protected void Start()
    {
        attachSceneUi();
        if (m_SceneSkybox != null)
        {
            RenderSettings.skybox = m_SceneSkybox.material;
        }
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
        yield return new WaitForSecondsRealtime(5.0f);
        GCanvasController.instance.removeFromCover(title);
    }

    // protected looper updatePreLoop;
    // protected checker checkPreState;
    // protected looper updateMainLoop;
    // protected checker checkMainState;
    // protected looper updateEndLoop;
    // protected checker checkEndState;

    // public void startGameLoop(){
    //     StartCoroutine(ieGameProcess());
    // }

    // protected delegate bool checker();

    // protected delegate void looper();

    // IEnumerator ieGameProcess(){
    //     if(checkPreState != null && updatePreLoop != null){
    //         yield return ieCheckAndLoop(checkPreState, updatePreLoop);
    //     }
    //     if(checkMainState != null && updateMainLoop != null){
    //         yield return ieCheckAndLoop(checkMainState, updateMainLoop);
    //     }
    //     if(checkEndState != null && updateEndLoop != null){
    //         yield return ieCheckAndLoop(checkEndState, updateEndLoop);
    //     }
    //     yield return null;
    //     backToMenu();
    // }

    // IEnumerator ieCheckAndLoop(checker chk, looper lop){
    //     while (chk())
    //     {
    //         lop();
    //         yield return null;
    //     }
    // }
}

[CustomEditor(typeof(SceneBaseController))]
class SceneBaseControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = target as SceneBaseController;
        if (GUILayout.Button("放置UI"))
        {
            script.attachSceneUi();
        }
    }
}
