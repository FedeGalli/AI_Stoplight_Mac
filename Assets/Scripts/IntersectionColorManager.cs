using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Coordinates{
    North,
    South,
    East,
    West,
    
}
public class IntersectionColorManager : MonoBehaviour
{
    private Transform[] intersections;
    public float yellowDuration;

    public float allRedDuration; //to allow the cars passed with the yellow to clear the intersection
    public bool timered;
    public float mainGreenDuration;
    public float otherGreenDuration;

    [HideInInspector]
    public Color green;
    [HideInInspector]
    public Color red;
    [HideInInspector]
    public Color yellow;

    void Start(){
        green = new Color(89f/255f,229f/255f,49f/255f,63f/255f); 
        red = new Color(255f/255f,0f/255f,7f/255f,63f/255f);
        yellow = new Color(255f/255f, 235f/255f, 16f/255f, 63f/255f);
        int arrayLength = 0;
        foreach(Transform t in GetComponentInChildren<Transform>()){
            if (t.name.Contains("_Intersection")){
                arrayLength ++;
            }
        }
        intersections = new Transform[arrayLength];
        int i = 0;
        foreach (Transform intersection in this.GetComponentsInChildren<Transform>()){
            if (intersection.name != this.name && intersection.name.Contains("Intersection")){
                intersections[i] = intersection;
                i++;
            }
        }

        initializeLights();
    }

    void initializeLights(){
        if (intersections.Length == 2){
            intersections[0].GetComponent<Renderer>().material.SetColor("_Color", green);
            intersections[1].GetComponent<Renderer>().material.SetColor("_Color", red);
            if (timered){
                StartCoroutine(timer());
            }
        }
        else if (intersections.Length == 4){
            intersections[0].GetComponent<Renderer>().material.SetColor("_Color", green);
            intersections[1].GetComponent<Renderer>().material.SetColor("_Color", green);       

            intersections[2].GetComponent<Renderer>().material.SetColor("_Color", red);
            intersections[3].GetComponent<Renderer>().material.SetColor("_Color", red);

            if (timered){
                StartCoroutine(timer());
            }
        }

    }

    public void activateOnDirection(Coordinates direction){
        if ((int)direction == 0){ //MAIN DIRECTION
            if (getColor(direction) == red) { //IF GREEN OR YELLOW DO NOTHING
                if (intersections.Length == 2)
                    stopLightLogic(0,1);
                else if (intersections.Length == 4)
                   stopLightLogic(0,1,2,3); 
            }
        }
        else{ //OTHER DIRECTION
            if (getColor(direction) == red) { //IF GREEN OR YELLOW DO NOTHING
                if (intersections.Length == 2)
                    stopLightLogic(1,0);
                else if (intersections.Length == 4)
                    stopLightLogic(2,3,0,1);
            }
        }  
    }

    public void stopLightLogic(int activationDirenction0, int activationDirenction1, int activationDirenction2, int activationDirenction3){           
        intersections[activationDirenction2].GetComponent<Renderer>().material.SetColor("_Color", yellow);
        intersections[activationDirenction3].GetComponent<Renderer>().material.SetColor("_Color", yellow);

        StartCoroutine(deactivateOtherDirection(activationDirenction0, activationDirenction1, activationDirenction2,activationDirenction3));

    }

    public void stopLightLogic(int activationDirenction0, int activationDirenction1){           
        intersections[activationDirenction1].GetComponent<Renderer>().material.SetColor("_Color", yellow);

        StartCoroutine(deactivateOtherDirection(activationDirenction0, activationDirenction1));

    }
    IEnumerator deactivateOtherDirection(int param0, int param1 ,int param2, int param3)
    {   
        //Wait for colorYellowDuration seconds
        yield return new WaitForSeconds(yellowDuration);

        intersections[param2].GetComponent<Renderer>().material.SetColor("_Color", red);
        intersections[param3].GetComponent<Renderer>().material.SetColor("_Color", red);

        yield return new WaitForSeconds(allRedDuration);
        intersections[param0].GetComponent<Renderer>().material.SetColor("_Color", green);
        intersections[param1].GetComponent<Renderer>().material.SetColor("_Color", green);
        
    }
    IEnumerator deactivateOtherDirection(int param0, int param1)
    {   
        //Wait for colorYellowDuration seconds
        yield return new WaitForSeconds(yellowDuration);

        intersections[param1].GetComponent<Renderer>().material.SetColor("_Color", red);
        yield return new WaitForSeconds(allRedDuration);
        intersections[param0].GetComponent<Renderer>().material.SetColor("_Color", green);
        
    }
    IEnumerator timer()
    {   
        //Wait for colorYellowDuration seconds
        int i=0;
        while(true){
            if (i!=0)
                yield return new WaitForSeconds(mainGreenDuration + yellowDuration);
            else
                yield return new WaitForSeconds(mainGreenDuration);
                if (intersections.Length == 4)
                    activateOnDirection(Coordinates.East);
                else if (intersections.Length == 2)
                    activateOnDirection(Coordinates.South);
            yield return new WaitForSeconds(otherGreenDuration + yellowDuration);
                activateOnDirection(Coordinates.North);
            i++;
        }
    }

    public Color getColor(Coordinates direction){
        return intersections[(int)direction].GetComponent<Renderer>().material.GetColor("_Color");
    }

    public void resetIntersectionLogic(){
        StopAllCoroutines();
        initializeLights();
    }

}
