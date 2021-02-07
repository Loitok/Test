using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTraffic : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    int time;
    void Start()
    {
        CheckRotation();
        StartCoroutine(ChangeLight());
        
    }
 
    void CheckRotation()
    {
        var rot = transform.rotation.eulerAngles.z;
        if (rot == 0) time = 0;
        else if (rot == 90) time = 4;
        else if (rot == 180) time = 8;
        else if (rot == 270) time = 12;
    }
    IEnumerator ChangeLight()
    {
        redLight.SetActive(true);
        yield return new WaitForSeconds(time);
        while (true)
        {
            greenLight.SetActive(true);
            redLight.SetActive(false);
            yield return new WaitForSeconds(4);
            yellowLight.SetActive(true);
            greenLight.SetActive(false);
            yield return new WaitForSeconds(2);
            redLight.SetActive(true);
            yellowLight.SetActive(false);
            yield return new WaitForSeconds(16);
        }
    }
}