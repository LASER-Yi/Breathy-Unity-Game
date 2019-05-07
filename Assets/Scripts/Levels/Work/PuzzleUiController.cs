using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUiController : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_ElementPrefab;

    private Canvas m_PuzzleUi;
    private List<RectTransform> m_PuzzleObject;
    private List<Transform> m_KeyboardObject;

    void Awake(){
        // 动态绑定键盘，生成UI元素
    }

    public void updatePuzzle(List<int> puzzle, int index)
    {

    }
}
