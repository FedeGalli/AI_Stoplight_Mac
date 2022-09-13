using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarManager : MonoBehaviour
{
    // Start is called before the first frame update
    public IntersectionColorManager intersection;
    public float speed;
    public Coordinates dir;
    private float safeDistance;
    private float maxCarSpeed;
    private float hurry;
    public float minSafeDistance;
    private float distanceFromNextCar;
    public Transform oppositeCarsContainer;
    public Vector3 stopPoint;
    public Vector3 destroyPoint;
    public bool isTurn;
    private float distanceTravelled;

    private Vector3 lookDirection;

    private bool passed;
    private int bitMask;
    private bool isGreen;
    private bool clearIntersection;
    private float timeAlive;
    private bool mustGo;
    private float yellowTime;
    
    private enum states{
        Throttle,
        Brake,
        Constant
    }
    
    void Start()
    {
        clearIntersection=false;
        passed=false;
        mustGo = false;
        safeDistance=0f;
        distanceTravelled = 0;
        hurry = Random.Range(1f, 2f);
        minSafeDistance = Random.Range(10, 10);
        
        bitMask = 1 << 6; //layer number 6
        timeAlive = 0;
    }


    void FixedUpdate(){
        timeAlive += Time.deltaTime;
        lookDirection = transform.TransformDirection(Vector3.forward); // check the yellowtime to let cars pass

        if (intersection.getColor(dir) == intersection.yellow){
            yellowTime += Time.deltaTime;
        }
        else {
            yellowTime = 0;
        }

        if (((intersection.getColor(dir) == intersection.red) || (yellowTime > intersection.yellowDuration /2 )) && !passed){
            //add the stop layer as a raycast trigger, when is red or the yellowtime is other the half of it
            bitMask = 1 << 6;
        }
        else{
            bitMask = -65; //all the layers except the stop point layer
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, lookDirection, out hit, Mathf.Infinity, bitMask | 1<<7) && !mustGo)
        {
            distanceFromNextCar = hit.distance;
        }
        else {
            distanceFromNextCar = 150f; //supose nothing in front
        }
        if (passed && this.GetComponent<SplineFollower>().getSpline().name.Contains("Left") && !mustGo){
            //the value 45 and 25 (for the distance from the stop point) are working well with this kind of set up, may change using other dimensions.
            float closestCar = 45f;
            foreach (Transform oppositeCar in oppositeCarsContainer.GetComponentInChildren<Transform>()){
                if (!oppositeCar.GetComponent<CarManager>().passed && Vector3.Distance(this.transform.position, oppositeCar.position) > 45f){
                    break;
                }
                if (Vector3.Distance(this.transform.position, oppositeCar.position) < closestCar && !oppositeCar.GetComponent<SplineFollower>().getSpline().name.Contains("Left") && oppositeCar.GetComponent<CarManager>().speed > 0){
                    //saving the closest car value just for cars approaching the veicle and not the already passed ones.
                    if ((oppositeCar.GetComponent<CarManager>().passed && Vector3.Distance(oppositeCar.transform.position, oppositeCar.GetComponent<CarManager>().stopPoint) < 25f) || (!oppositeCar.GetComponent<CarManager>().passed)){
                        closestCar = Vector3.Distance(this.transform.position, oppositeCar.position);
                    }
                }

            }

            if (closestCar == 45f) { //if there is no car on the way, turn left and do not stop
                distanceFromNextCar = 150f;
                mustGo = true; //must go do not check for approaching veicle and push the car outside the intersection whatever.
                maxCarSpeed = 50/3.6f;
                hurry = 2f;
            }
            else
                distanceFromNextCar = 0f;
        }
        if (speed < minSafeDistance)
            safeDistance = minSafeDistance;
        else
            safeDistance = speed;
        
        carLogic();

        if (intersection.getColor(dir) == intersection.yellow && passed) //clear intersection fast
            clearIntersection = true;

    } 

    void carLogic(){
        if (distanceFromNextCar <= safeDistance && !clearIntersection){
            updateCarMovement(states.Brake);
        }
        else if (speed < maxCarSpeed) {
            updateCarMovement(states.Throttle);
        }
        else{
            updateCarMovement(states.Constant);
        }
    }

    void updateCarMovement(states state){
        if (state == states.Throttle){
            if (clearIntersection)
                safeDistance = minSafeDistance;
            if (speed + 0.02f > maxCarSpeed){
                speed=maxCarSpeed;
            }else{
                speed += 0.02f;
            }
            

        }
        else if (state == states.Brake){
            if (speed -0.04 < 0f){
                speed=0;
            }
            else{
                speed -= 0.04f;
            }      
        }

        distanceTravelled += speed * Time.deltaTime;
        this.GetComponent<SplineFollower>().setSpeed(speed);
    }

    void OnTriggerEnter(Collider other){
        if (other.name.Contains("Destroy_Point")){
            
            
            if (transform.parent.parent.name.Substring(14).Contains("0")){
                using(StreamWriter writetext = new StreamWriter("AIResults.txt", true))
                {
                    writetext.WriteLine(timeAlive);
                }  
            }
            else{
                
                using(StreamWriter writetext = new StreamWriter("StandardResults.txt", true))
                {
                    writetext.WriteLine(timeAlive);
                } 
            }
            
            
            Destroy(this.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.name.Contains("Stop_Point")){
            passed=true;

        }
    }

    public bool getIsPassed(){
        return passed;
    }

    public float getMaxCarSpeed(){
        return this.maxCarSpeed;
    }

    public void setMaxCaSpeed(){
        this.maxCarSpeed = Random.Range(30f / 3.6f, 50f / 3.6f); //velocity expressed in m/s
    }
    
}
