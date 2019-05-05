using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCarGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_CarList;

    // TODO: Car Info
    public void createCar(RoadChunk road){
        var rand = Random.Range(0f, 1f);
        if (m_CarList.Count > 0)
        {
            var roadNum = road.getRoadNum();
            var roadPicker = Random.Range(0, roadNum);
            var position = road.computeRoadCenterWorld(roadPicker);
            position.y = transform.position.y;

            var rotation = road.transform.rotation;

            var carPicker = Random.Range(0, m_CarList.Count);
            var carPrefab = m_CarList[carPicker];
            var car = Instantiate(carPrefab, position, rotation, transform.root);
            var script = car.GetComponent<CarController>();
            script.setEnginePower(0.5f);
        }
    }

    void Start(){
        createCarLoop(0.04f);
    }

    public void createCarLoop(float chance){
        StopAllCoroutines();
        StartCoroutine(ieCreateLoop(chance));
    }

    private RoadChunk checkBelowChunk(){
        var roadLayer = 1 << LayerMask.NameToLayer("Road");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, roadLayer))
        {
            var road = hit.transform.GetComponentInParent<RoadChunk>();
            return road;
        }
        return null;
    }

    IEnumerator ieCreateLoop(float chance){
        while (true)
        {
            yield return new WaitForSeconds(1f);
            var road = checkBelowChunk();
            if(road != null){
                createCar(road);
            }
        }
    }
}
