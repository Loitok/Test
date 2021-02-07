using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawningCars : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] spawnPoints;

    public float waitingForNextSpawn = 1;
    public float theCountdown = 0;
    private int i;

    public void Update()
    {
        var a = GameObject.FindGameObjectsWithTag("Car").Length;
        if (a > 10)
            Destroy(gameObject);
        theCountdown -= Time.deltaTime;
        if (theCountdown <= 0)
        {
            var point = SpawnRandom();
            while(!FindingNearestCar())
                FindingNearestCar();
            var car = GetCar();
            GameObject c = Instantiate(car, point.transform.position, point.transform.rotation);
            theCountdown = waitingForNextSpawn;
        }
    }

    bool FindingNearestCar()
    {
        var cars = GameObject.FindGameObjectsWithTag("Car");
        if (cars.Length == 0)
            return true;
        Dictionary<int, float> allDist = new Dictionary<int, float>(cars.Length);
        var pos = transform.position;

        for (int i = 0; i < cars.Length; i++)
        {
            var carPos = cars[i].transform.position;
            allDist.Add(i, Vector3.Distance(carPos, transform.position));
        }
        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;
        if (dictSort.ElementAt(0).Value < 0.49f)
            return false;
        else return true;
    }
    GameObject SpawnRandom()
    {
        i = Random.Range(0, spawnPoints.Length);
        return spawnPoints[i];
    }

    GameObject GetCar()
    {
        if (i % 2 == 0)
            return cars[0];
        return cars[1];
    }
}
