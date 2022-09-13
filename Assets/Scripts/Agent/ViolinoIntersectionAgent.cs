using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System.IO;

public class ViolinoIntersectionAgent : Agent
{
    // Start is called before the first frame update
    private CarFlowManager carFlowManager;
    private IntersectionColorManager colorManager;
    private float greenTime;
    private float maxSightDistance;
    private Coordinates greenDirection;
    private Transform[] carsContainer;
    private Coordinates prevCoordinate;
    private int stepsCount;
    private int carsPassed;
    private int[] carCount;
    private int episodeCount = 0;
    private float waitTime;
    private bool listenForReset;

    public override void Initialize()
    {
        int arrayLength = 0;
        foreach(Transform t in GetComponentInChildren<Transform>()){
            if (t.name.Contains("_Intersection")){
                arrayLength ++;
            }
        }
        //just use the line above to test the ai model with a standard model
        //Random.InitState(episodeCount);
        
        InvokeRepeating("flowIntesitivityRandomUpdate", 0f, Random.Range(30f,120f));
        colorManager = this.GetComponent<IntersectionColorManager>();
        carFlowManager = this.GetComponent<CarFlowManager>();
        maxSightDistance = 70f; // expressed in m
        carCount = new int[arrayLength];
        carsContainer = new Transform[arrayLength];
        for (int i =0; i<carsContainer.Length; i++){
            string directionName = (Coordinates)i + "_";
            carsContainer[i] = GameObject.Find(this.name+"/"+directionName + "Car_Container").GetComponent<Transform>();
            carCount[i] = 0;
        }
    }

    public override void OnEpisodeBegin(){
        resetScene();
        listenForReset = false;
        greenTime = 0;
        stepsCount = 0;
        carsPassed=0;
        waitTime = 0;
        greenDirection = Coordinates.North;
        prevCoordinate = Coordinates.North;

        
    }

    public override void CollectObservations(VectorSensor sensor){
        int count = 0;
        for (int i = 0; i<carsContainer.Length; i++){
            //Giving the amount of cars for each intersection in sight
                count = 0;
            foreach(Transform car in carsContainer[i].GetComponentsInChildren<Transform>()){
                if (car.name.Contains("Clone") && Vector3.Distance(car.transform.position, carFlowManager.getStopPoint(i).position) < maxSightDistance && !car.GetComponent<CarManager>().getIsPassed())
                    count+=1;
            }
            carCount[i] = count;
            
        }
        sensor.AddObservation(carCount[0]);
        sensor.AddObservation(carCount[1]);
        sensor.AddObservation(waitTime);
        sensor.AddObservation((int)greenDirection);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if ((colorManager.getColor(Coordinates.North) == colorManager.green && colorManager.getColor(Coordinates.South) == colorManager.red) || (colorManager.getColor(Coordinates.South) == colorManager.green && colorManager.getColor(Coordinates.North) == colorManager.red)){
            if (actions.DiscreteActions[0] == 0 && prevCoordinate != Coordinates.North){
                colorManager.activateOnDirection(Coordinates.North);
                prevCoordinate = Coordinates.North;
                greenDirection = Coordinates.North;
                AddReward(-0.5f);
            }
            else if (actions.DiscreteActions[0] == 1 && prevCoordinate != Coordinates.South){
                colorManager.activateOnDirection(Coordinates.South);
                prevCoordinate = Coordinates.South;
                greenDirection = Coordinates.South;
                AddReward(-0.5f);
            }
        }   

    }
    void FixedUpdate()
    {
        greenTime += Time.deltaTime;
        stepsCount++;
        if (colorManager.getColor(Coordinates.North) == colorManager.yellow || colorManager.getColor(Coordinates.South) == colorManager.yellow) {
            listenForReset = true;
        }
        if (listenForReset && colorManager.getColor(greenDirection) == colorManager.green){
            greenTime = 0;
            waitTime = 0;
            listenForReset = false;
        }
        
        if (colorManager.getColor(Coordinates.North) != colorManager.red) {
            if (carCount[1]!= 0){ //set a threshold to decide when is appropriate to give a constant negative reward
                AddReward((-0.00001f * (carCount[1]) * waitTime));
                waitTime += Time.deltaTime;
            }
        }
        if (colorManager.getColor(Coordinates.South) != colorManager.red) {
            if (carCount[0] != 0){ //set a threshold to decide when is appropriate to give a constant negative reward
                AddReward((-0.00001f * (carCount[0]) * waitTime));
                waitTime += Time.deltaTime;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.UpArrow)){
            greenDirection = Coordinates.North;
        }
        else if (Input.GetKey(KeyCode.DownArrow)){
            greenDirection = Coordinates.South;
        }

        if (greenDirection == Coordinates.North){
            discreteActionsOut[0] = 0;

        }else {
            discreteActionsOut[0] = 1;
        }
    }

    void OnTriggerExit(Collider other){
        if (other.transform.name.Contains("base"))
            AddReward(1f);
            carsPassed++;
    }

    void resetScene(){
        carFlowManager.resetSpawnPoint();
        colorManager.resetIntersectionLogic();
        for (int i = 0; i<carsContainer.Length; i++){
            foreach(Transform car in carsContainer[i].GetComponentsInChildren<Transform>()){
                if (car.gameObject.layer == 7)
                    Destroy(car.gameObject);
            }
        }
    }

    void flowIntesitivityRandomUpdate(){
        carFlowManager.resetSpawnPoint();
        //just use the 2 lines above to test the ai model with a standard model
        //episodeCount++;
        //Random.InitState(episodeCount);

        carFlowManager.flowIntesitivity[0] = 0.3f + Random.Range(-0.40f, 0.15f);
        carFlowManager.flowIntesitivity[1] = 0.3f + Random.Range(-0.40f, 0.15f);
    }

    public float getGreenTime(){
        return this.greenTime;
    }
}
