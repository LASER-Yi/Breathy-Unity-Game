using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;
using LGameStructure;

public class WorkSceneManager : SceneBaseController, ITimeDidChangedHandler
{

    private WorkSceneParam m_SceneParam;

    void initalCameraPosition()
    {
        var position = GameObject.FindGameObjectWithTag("Player").transform.position;
        CameraAttribute attr = CameraAttribute.Empty;
        attr.position = position;
        attr.rotation = Quaternion.identity;
        attr.zlength = 0f;
        attr.fov = 50f;
        m_CamController.setAttribute(attr);
    }
    new void Start()
    {
        base.Start();
        initalCameraPosition();
        m_Game.setDeltaClock(0.1f);
        m_Game.setTimeSpeed(12f);
        m_Game.startTimeLoop();
        m_Game.addEventListener(this);
    }

    void OnDestroy()
    {
        m_Game.removeEventListener(this);
    }

    public void OnGameTimeChanged(GameManager sender, TimeOfGame time)
    {
        if (time.hour >= 20)
        {
            // 结束工作
            GSceneController.instance.LoadNextScene(true);
        }
    }

    List<int> randomElement(List<int> list)
    {
        var originList = new List<int>(list);
        var randList = new List<int>(list.Count);
        while (originList.Count != 0)
        {
            var pick = Random.Range(0, originList.Count);
            randList.Add(originList[pick]);
            originList.RemoveAt(pick);
        }
        return randList;
    }

    public void solveCurrentPuzzle(){
        // 给玩家增加金钱
    }

    private int m_MaxLimiter = 9;

    public KeyValuePair<int, List<int>> computePuzzle()
    {
        Random.InitState(Mathf.CeilToInt(Time.realtimeSinceStartup * 1000));
        var answer = Random.Range(m_MaxLimiter - 4, m_MaxLimiter);
        ++m_MaxLimiter;

        var puzzle = new List<int>();
        puzzle.Add(answer);
        var step = 0;
        while (puzzle.Count < 9)
        {
            step += Random.Range(1, 3);
            puzzle.Add(answer + step);
            puzzle.Add(answer - step);
        }

        var randPuzzle = randomElement(puzzle);

        var result = new KeyValuePair<int, List<int>>(answer, randPuzzle);
        return result;
    }
}
