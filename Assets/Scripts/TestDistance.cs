using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDistance : MonoBehaviour
{
    // Start is called before the first frame update
    public float distance = 0;
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.forward * 10 * Time.deltaTime);
    }

    void OnTriggerStay(Collider other){
        distance += 10f * Time.deltaTime;
    }
    void OnTriggerExit(Collider other){
        Debug.Log(distance);
    }
}
