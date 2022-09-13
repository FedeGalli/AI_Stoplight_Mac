using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TextShow : MonoBehaviour
{
    // Start is called before the first frame update
    private string textValue;
    private Text textElement;

    public IntersectionAgent intersection;

    void Start()
    {
        textElement = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textValue = "Green time: " + Math.Round(intersection.getGreenTime(),2);

        textElement.text = textValue;
    }
}
