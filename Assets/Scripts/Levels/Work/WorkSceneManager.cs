using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class WorkSceneManager : SceneBaseController
{
    private CameraController m_CamController
    {
        get
        {
            return CameraController.instance;
        }
    }

    private int m_SolveCounter = 0;

    public int getSolveCounter(){
        return m_SolveCounter;
    }

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
        GameManager.instance.setTimeOfDay(9, 30);
        GameManager.instance.startDayLoop();
    }

    void Update(){
        if(GameManager.instance.getTimeOfDayOriginal() > 9f){
            // 结束工作
        }
    }

    List<int> randomElement(List<int> list){
        var originList = new List<int>(list);
        var randList = new List<int>(list.Count);
        while(originList.Count != 0){
            var pick = Random.Range(0, originList.Count);
            randList.Add(originList[pick]);
            originList.RemoveAt(pick);
        }
        return randList;
    }

    private int m_MaxLimiter = 9;

    public KeyValuePair<int, List<int>> computePuzzle()
    {
        ++m_SolveCounter;
        Random.InitState(Mathf.CeilToInt(Time.realtimeSinceStartup * 1000));
        var answer = Random.Range(5, m_MaxLimiter);
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
