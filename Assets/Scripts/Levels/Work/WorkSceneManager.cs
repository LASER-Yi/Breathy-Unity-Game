using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCameraSystem;

public class WorkSceneManager : SceneBaseController
{
    private CameraController m_CamController{
        get{
            return CameraController.instance;
        }
    }

    void initalCameraPosition(){
        var position = GameObject.FindGameObjectWithTag("Player").transform.position;   
        CameraAttribute attr = CameraAttribute.Empty;
        attr.position = position;
        attr.rotation = Quaternion.identity;
        attr.zlength = 0f;
        attr.fov = 50f;
        m_CamController.setAttribute(attr);
    }
    new void Start(){
        base.Start();
        initalCameraPosition();
        GameManager.instance.setTimeOfDay(9, 30);
        GameManager.instance.startDayLoop();
    }

    public List<int> puzzleGenerator(){
        var puzzle = new List<int>();
        puzzle.AddRange(System.Linq.Enumerable.Range(0, 9));
        return puzzle;
    }
}
