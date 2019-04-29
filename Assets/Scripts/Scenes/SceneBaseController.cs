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

    protected CameraController m_Controller
    {
        get
        {
            return CameraController.instance;
        }
    }

    protected UiCanvasController m_UiController
    {
        get
        {
            return UiCanvasController.instance;
        }
    }

    [SerializeField]
    private bool m_IsShowAfterLoad = false;
    private string m_SceneName;
    [SerializeField]
    private RectTransform m_TitlePrefab;

    [SerializeField]
    private Skybox m_SceneSkybox;

    public void setSceneName(string s){
        m_SceneName = s;
    }
    public string getSceneName()
    {
        return m_SceneName;
    }

    public Skybox getSceneSkybox()
    {
        return m_SceneSkybox;
    }

    public void backToMenu(){
        SceneController.instance.LoadSceneAsync(SceneController.ESceneIndex.Menu);
    }

    protected void Start()
    {
        if (m_IsShowAfterLoad && m_TitlePrefab != null)
        {
            StartCoroutine(IE_ShowTitle());
        }
    }

    IEnumerator IE_ShowTitle()
    {
        var title = UiCanvasController.instance.directOverlay(m_TitlePrefab);
        var text = title.GetComponent<UnityEngine.UI.Text>();
        if (text != null) text.text = m_SceneName;
        yield return new WaitForSecondsRealtime(3.0f);
        Destroy(title.gameObject);
    }
}
