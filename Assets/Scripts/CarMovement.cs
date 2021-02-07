using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private GameObject target;
    private GameObject[] checkPoints;
    private Vector3 carMoving;
    private Vector3 newTarg;
    private bool rightTurn;
    private bool firstTurn;
    private bool stopMoving;
    private bool stopTrafficMoving;

    void Start()
    {
        Color newColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        gameObject.GetComponent<Renderer>().material.color = newColor;
        target = FindingNearestTrafficStop();
        checkPoints = GameObject.FindGameObjectsWithTag("CheckPoint");
    }

    void Update()
    {
        float step = 1.2f * Time.deltaTime;

        var playerPos = transform.position;
        var targetPos = target.transform.position;

        FindCar();
        FindTrafficLight();
        MaybeDestroy();

        if (!stopMoving && !stopTrafficMoving)
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, step);
        
        if (playerPos == targetPos)
        {
            if (target.tag == "TrafficLightStop" && target.name != "IntersectionCheckpoint")
            {
                carMoving = target.transform.position;
                target = TrafficStop();
            }

            else if (target.tag == "CheckPoint")
            {
                if (rightTurn)
                {
                    if (carMoving.x == playerPos.x && carMoving.y > playerPos.y)
                    {
                        if (!firstTurn)
                            newTarg = target.transform.position + new Vector3(2.75f, 0, 0);
                        else
                        {
                            newTarg = target.transform.position + new Vector3(-2.75f, 0, 0);
                            firstTurn = false;
                        }
                    }

                    else if (carMoving.x == playerPos.x && carMoving.y < playerPos.y)
                    {
                        if (!firstTurn)
                            newTarg = target.transform.position + new Vector3(-2.75f, 0, 0);
                        else
                        {
                            newTarg = target.transform.position + new Vector3(2.75f, 0, 0);
                            firstTurn = false;
                        }
                    }

                    else if (carMoving.x > playerPos.x && carMoving.y == playerPos.y)
                        newTarg = target.transform.position + new Vector3(0, 2.75f, 0);
                    
                    else if (carMoving.x < playerPos.x && carMoving.y == playerPos.y)
                        newTarg = target.transform.position + new Vector3(0, -2.75f, 0);
                    
                    target.transform.position = newTarg;
                    target.tag = "TrafficLightStop";
                    Rotation();
                    rightTurn = false;
                }
                else
                {
                    target = FindNext();
                    Rotation();
                }
            }

            else if(target.tag == "TrafficLightStop" && target.name == "IntersectionCheckpoint")
            {
                rightTurn = false;
                carMoving = target.transform.position;
                target.transform.position -= newTarg;
                target.tag = "CheckPoint";
                target = TrafficStop();
                Rotation();
            }
        }
    }

    GameObject FindNext()
    {
        var trafficStops = GameObject.FindGameObjectsWithTag("TrafficLightStop");
        Dictionary<int, float> allDist = new Dictionary<int, float>(trafficStops.Length);
        var playerPos = transform.position;

        for (int i = 0; i < trafficStops.Length; i++)
        {
            var trafficStopPos = trafficStops[i].transform.position;
            if (playerPos.x == trafficStopPos.x || playerPos.y == trafficStopPos.y)
                if(Vector3.Distance(trafficStopPos, transform.position) > 1.25f && Vector3.Distance(trafficStopPos, transform.position) < 4f)
                    allDist.Add(i, Vector3.Distance(trafficStopPos, transform.position));
        }
        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;

        if (dictSort.Count() > 1)
        {
            var a = UnityEngine.Random.Range(0, 2);
            playerPos = transform.position;

            return trafficStops[dictSort.ElementAt(a).Key];
     
        }
        else
            return trafficStops[dictSort.ElementAt(0).Key];
    }

    GameObject TrafficStop()
    {
        Dictionary<int, float> allDist = new Dictionary<int, float>(checkPoints.Length);
        var playerPos = transform.position;
        for (int i = 0; i < checkPoints.Length; i++)
        {
            var checkPointPos = checkPoints[i].transform.position;
            if (playerPos.x == checkPointPos.x || playerPos.y == checkPointPos.y)
                allDist.Add(i, Vector3.Distance(checkPoints[i].transform.position, transform.position));
        }

        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;
           
        if (Math.Abs(dictSort.ElementAt(0).Value - dictSort.ElementAt(1).Value) < 1)
        {
            int randomChoice = UnityEngine.Random.Range(0, 2);
            if (randomChoice == 0)
            {
                rightTurn = true;
                firstTurn = true;
            }
            return checkPoints[dictSort.ElementAt(randomChoice).Key];
        }
        else
            return checkPoints[dictSort.ElementAt(0).Key];
    }

    GameObject FindingNearestTrafficStop()
    {
        var trafficStops = GameObject.FindGameObjectsWithTag("TrafficLightStop");
        Dictionary<int, float> allDist = new Dictionary<int, float>(trafficStops.Length);
        var playerPos = transform.position;

        for (int i = 0; i < trafficStops.Length; i++)
        {
            var trafficStopPos = trafficStops[i].transform.position;
            if (playerPos.x == trafficStopPos.x || playerPos.y == trafficStopPos.y)
                allDist.Add(i, Vector3.Distance(trafficStopPos, transform.position));
        }
        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;
        return trafficStops[dictSort.ElementAt(0).Key];
    }

    void Rotation()
    {
        if (carMoving.x != target.transform.position.x && carMoving.y != target.transform.position.y)
        {
            var rotationVector = transform.rotation.eulerAngles;
            if (rotationVector.z == 90)
                rotationVector.z = 0;
            else rotationVector.z = 90;
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }

    void MaybeDestroy()
    {
        if (Math.Abs(transform.position.x) > 6 || Math.Abs(transform.position.y) > 6)
            Destroy(gameObject);
    }

    void FindCar()
    {
        var cars = GameObject.FindGameObjectsWithTag("Car");
        Dictionary<int, float> allDist = new Dictionary<int, float>(cars.Length);
        var playerPos = transform.position;

        for (int i = 0; i < cars.Length; i++)
        {
            var carPos = cars[i].transform.position;
            if (carPos != transform.position)
                allDist.Add(i, Vector3.Distance(carPos, transform.position));
        }
        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;
        if (dictSort.Count() != 0 && dictSort.ElementAt(0).Value < 0.49f && !stopTrafficMoving
            && dictSort.ElementAt(1).Value - dictSort.ElementAt(0).Value > 0.1f && 
            cars[dictSort.ElementAt(0).Key].GetComponent<CarMovement>().stopTrafficMoving
            && Vector3.Distance(cars[dictSort.ElementAt(0).Key].transform.position, 
            cars[dictSort.ElementAt(1).Key].transform.position) < 1.1f && dictSort.ElementAt(1).Value < 1.1f)
        {
            stopMoving = true;
        }
        else stopMoving = false;
    }

    void FindTrafficLight()
    {
        var trafficLights = GameObject.FindGameObjectsWithTag("TrafficLight");
        Dictionary<int, float> allDist = new Dictionary<int, float>(trafficLights.Length);
        var playerPos = transform.position;

        for (int i = 0; i < trafficLights.Length; i++)
        {
            var trLightPos = trafficLights[i].transform.position;
            if (trLightPos != transform.position)
                allDist.Add(i, Vector3.Distance(trLightPos, transform.position));
        }
        var dictSort = from objDict in allDist orderby objDict.Value ascending select objDict;
        if ((dictSort.ElementAt(0).Value < 0.48f && trafficLights[dictSort.ElementAt(0).Key].GetComponent<LightTraffic>().redLight.active)
          ||  (dictSort.ElementAt(0).Value < 0.48f && trafficLights[dictSort.ElementAt(0).Key].GetComponent<LightTraffic>().yellowLight.active))
        {
            stopTrafficMoving = true;
        }
        else stopTrafficMoving = false;
    }
}
