using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUiController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_PuzzlePrefab;
    [SerializeField]
    private Transform m_KeyboardHolder;
    private Canvas m_PuzzleUi;
    private List<Transform> m_KeyboardObjects;
    private List<Text> m_PuzzleObjects;
    void Awake()
    {
        // 动态绑定键盘，生成UI元素
        m_PuzzleUi = GetComponent<Canvas>();
        foreach (Transform item in m_PuzzleUi.transform)
        {
            Destroy(item.gameObject);
        }
        m_PuzzleObjects = new List<Text>(9);
        m_KeyboardObjects = new List<Transform>(9);

        foreach (var index in System.Linq.Enumerable.Range(0, 9))
        {
            var key = m_KeyboardHolder.GetChild(index);
            m_KeyboardObjects.Add(key);

            var puzzle = Instantiate(m_PuzzlePrefab);
            puzzle.SetParent(m_PuzzleUi.transform, false);
            var puzText = puzzle.GetComponent<Text>();
            puzText.text = "";
            m_PuzzleObjects.Add(puzText);
        }
    }

    private WorkSceneManager m_Controller;

    void Start()
    {
        if (SceneBaseController.instance is WorkSceneManager ws)
        {
            m_Controller = ws;
        }
        StartCoroutine(ieScreenLoop());
    }

    void Update()
    {
        updateUserInput();
    }

    private KeyCode[] m_DetectKey = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Z, KeyCode.X, KeyCode.C };

    void updateUserInput()
    {
        if(Input.GetButton("Jump")) return;
        int? pressKey = null;
        for (int index = 0; index < m_DetectKey.Length; ++index)
        {
            if (Input.GetKeyUp(m_DetectKey[index]))
            {
                pressKey = index;
                break;
            }
        }
        if (pressKey is int pIndex)
        {
            handleKeyPress(pIndex);
        }
    }

    private List<int> m_PressKey = new List<int>();

    private List<int> m_CorrectList = new List<int>();

    void handleKeyPress(int index)
    {
        if (m_CorrectList.Contains(index)) return;
        m_PressKey.Add(index);

        if (m_PressKey.Count == 2)
        {
            foreach (var saveIndex in m_PressKey)
            {
                updateVisualEffect(saveIndex, EEffectType.Default);
            }
            checkPuzzleCorrect();
            m_PressKey.Clear();
        }
        else
        {
            updateVisualEffect(index, EEffectType.Active);
        }
    }

    enum EEffectType
    {
        Active,
        Default,
        Correct
    }

    void updateVisualEffect(int index, EEffectType type)
    {
        switch (type)
        {
            case EEffectType.Active:
                {
                    m_PuzzleObjects[index].color = Color.red;
                    break;
                }
            case EEffectType.Default:
                {
                    m_PuzzleObjects[index].color = Color.white;
                    break;
                }
            case EEffectType.Correct:
                {
                    m_PuzzleObjects[index].color = Color.clear;
                    break;
                }
        }
    }

    void checkPuzzleCorrect()
    {
        var first = m_PressKey[0];
        var last = m_PressKey[1];
        if (first == last) return;

        if (m_PuzzleList[first] + m_PuzzleList[last] == m_PuzzleResult * 2)
        {
            m_CorrectList.Add(first);
            m_CorrectList.Add(last);
            updateVisualEffect(first, EEffectType.Correct);
            updateVisualEffect(last, EEffectType.Correct);
            checkPuzzleAllSolve();
        }
    }

    void checkPuzzleAllSolve()
    {
        if (m_CorrectList.Count == 8)
        {
            m_IsPuzzleSolve = true;
            m_Controller.solveCurrentPuzzle();
        }
    }

    private List<int> m_PuzzleList;
    private int m_PuzzleResult;

    private bool m_IsPuzzleSolve = false;

    private IEnumerator findNewPuzzle()
    {
        var puzzleBox = m_Controller.computePuzzle();
        yield return resetPuzzle(puzzleBox.Value, puzzleBox.Key);
    }

    private IEnumerator resetPuzzle(List<int> ques, int result)
    {
        for (int i = 0; i < m_PuzzleObjects.Count; ++i)
        {
            m_PuzzleObjects[i].text = ques[i].ToString();
            updateVisualEffect(i, EEffectType.Default);
            yield return new WaitForSeconds(0.1f);
        }
        m_PuzzleList = ques;
        m_PuzzleResult = result;
        m_CorrectList.Clear();
        m_PressKey.Clear();
        m_IsPuzzleSolve = false;
    }

    IEnumerator ieScreenLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            yield return findNewPuzzle();
            yield return new WaitUntil(() => m_IsPuzzleSolve);
        }
    }
}
