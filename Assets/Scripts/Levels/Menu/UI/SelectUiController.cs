using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// TODO: 方块内随机
// 左右分层
// 线连接
class SelectUiController : MonoBehaviour, IStackableUi
{
    private struct SelectInfo
    {
        // public LineRenderer line;
        public Vector3 pointPosition;
        public Button bindBtn;
        public GSceneController.ESceneIndex bindScene;
    }

    private MenuSceneManager m_Manager;

    private GameObject _lineContainer;
    private GameObject m_LineContainer
    {
        get
        {
            if (_lineContainer == null)
            {
                var _obj = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                _obj.transform.SetParent(transform);
                _obj.name = "__line__container__";
                _lineContainer = _obj;
            }
            return _lineContainer;
        }
    }


    [SerializeField]
    private Button m_BtnPrefab;
    [SerializeField]
    private LineRenderer m_LinePrefab;
    [SerializeField]
    private float m_AxisXOffset = 100f;
    [SerializeField]
    private Vector2 m_BoxBound;
    [SerializeField]
    private Vector3 m_CenterPoint = Vector3.zero;

    List<SelectInfo> m_SceneInfo;

    private Canvas m_Canvas;

    public void setupManager(MenuSceneManager instance){
        m_Manager = instance;
    }

    Vector3 generateWorldPositionInBox()
    {
        var position = Vector3.one;
        position.x *= Random.Range(-1f, 1f);
        position.y *= Random.Range(-1f, 1f);
        position.z *= Random.Range(-1f, 1f);
        return position;
    }

    void initalSceneInfo()
    {
        if (m_BtnPrefab == null) return;

        m_SceneInfo = new List<SelectInfo>();
        var sceneList = GSceneController.instance.getSceneList();
        foreach (var item in sceneList)
        {
            var select = new SelectInfo();
            select.bindScene = item.Key;

            var btn = Instantiate(m_BtnPrefab).GetComponent<Button>();
            btn.transform.SetParent(transform);
            btn.transform.localPosition = Vector3.zero;
            btn.transform.localScale = Vector3.one;
            select.bindBtn = btn;
            var btnText = btn.GetComponentInChildren<Text>();
            btnText.text = item.Value;
            btn.onClick.AddListener(delegate { onBtnClick(select.bindScene); });

            select.pointPosition = generateWorldPositionInBox();

            // var line = Instantiate(m_LinePrefab);
            // select.line = line;
            // line.transform.SetParent(m_LineContainer.transform, false);

            m_SceneInfo.Add(select);
        }
    }

    void onBtnClick(GSceneController.ESceneIndex index)
    {
        m_Manager.transToSubGame(index);
    }

    void Awake()
    {
        initalSceneInfo();
        m_Canvas = GetComponentInParent<Canvas>();
    }

    void updateSelectInfo()
    {
        var centerPoint = Vector2.zero;
        foreach (var item in m_SceneInfo)
        {
            var screenPoint = Camera.main.WorldToScreenPoint(item.pointPosition - m_CenterPoint);
            centerPoint.x += screenPoint.x;
            centerPoint.y += screenPoint.y;
        }

        centerPoint /= (float)m_SceneInfo.Count;
        centerPoint = RectTransformUtility.PixelAdjustPoint(centerPoint, transform, m_Canvas);

        foreach (var item in m_SceneInfo)
        {
            var _point = Camera.main.WorldToScreenPoint(item.pointPosition);
            var screenPoint = new Vector2(_point.x, _point.y);
            screenPoint = RectTransformUtility.PixelAdjustPoint(screenPoint, item.bindBtn.transform, m_Canvas);

            var depth = _point.z;

            var offset = screenPoint - centerPoint;
            offset = offset.normalized;
            offset.x = Mathf.Lerp(-m_BoxBound.x, m_BoxBound.x, offset.x);
            offset.y = Mathf.Lerp(-m_BoxBound.y, m_BoxBound.y, offset.y);
            // 分层
            offset.x += (offset.x < 0 ? -m_AxisXOffset : m_AxisXOffset);

            var current = item.bindBtn.GetComponent<RectTransform>();
            current.localPosition = Vector2.Lerp(current.localPosition, offset, 0.1f);

        }
    }

    void Update()
    {
        updateSelectInfo();
    }

    void OnDestroy()
    {
        if (_lineContainer != null) Destroy(_lineContainer);
    }

    public RectTransform getTransform(){
        return transform as RectTransform;
    }
    public void onDidPushToStack(bool animate){

    }
    public void onDidBecomeTop(){

    }
    public void onWillNotBecomeTop(){

    }
    public float onWillRemoveFromStack(bool animate){
        return 0f;
    }

}