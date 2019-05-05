using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCarGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_CarList;

    public void generateCar(RoadChunk road){
        var rand = Random.Range(0f, 1f);
        if (m_CarList.Count > 0)
        {
            Debug.Log("Generating Car...");
            var roadNum = road.getRoadNum();
            var roadPicker = Random.Range(0, roadNum);
            var position = road.computeRoadCenterWorld(roadPicker);
            position.y = transform.position.y;

            var rotation = road.transform.rotation;

            var carPicker = Random.Range(0, m_CarList.Count);
            var carPrefab = m_CarList[carPicker];
            var car = Instantiate(carPrefab, position, rotation, transform);
            var script = car.GetComponent<CarController>();
            script.setEnginePower(0.5f);
        }
    }
}
