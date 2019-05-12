using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCarGenerator : MonoBehaviour
{
    private RoadController m_RController;
    [SerializeField]
    private List<GameObject> m_CarList;

    [SerializeField]
    private float m_CreateChance = 0.7f;

    void Awake()
    {
        m_RController = GetComponent<RoadController>();
    }

    void Start()
    {
        startCreateLoop();
    }

    // TODO: Car Info
    public void generateCar(RoadChunk chunk)
    {
        var rand = Random.Range(0f, 1f);
        if (m_CarList.Count > 0)
        {
            var roadNum = chunk.getRoadNum();
            var roadPicker = Random.Range(0, roadNum);
            var position = chunk.computeRoadCenterWorld(roadPicker);
            position.y = transform.position.y;

            var rotation = chunk.transform.rotation;

            var carPicker = Random.Range(0, m_CarList.Count);
            var carPrefab = m_CarList[carPicker];
            var car = Instantiate(carPrefab, position, rotation, transform.root);
            var script = car.GetComponent<CarController>();
            script.setEnginePower(0.7f);
        }
    }

    private RoadChunk pickRandomRoad()
    {
        var roadCount = m_RController.getRoadCount();
        var roadPicker = Random.Range(0, roadCount);
        var currentRoad = m_RController.getLastRoadNode();
        while (roadPicker > 0)
        {
            currentRoad = currentRoad.getFrontRoad();
            --roadPicker;
        }

        var chunk = currentRoad.getRandomChunk();
        var screenPoint = Camera.main.WorldToScreenPoint(chunk.transform.position);
        if (screenPoint.y < 0 || screenPoint.y > Screen.height)
        {
            return chunk;
        }
        return pickRandomRoad();
    }



    public void startCreateLoop()
    {
        StopAllCoroutines();
        StartCoroutine(ieCreateLoop());
    }

    IEnumerator ieCreateLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (Random.Range(0f, 1f) < m_CreateChance)
            {
                var chunk = pickRandomRoad();
                generateCar(chunk);
            }
        }
    }
}
