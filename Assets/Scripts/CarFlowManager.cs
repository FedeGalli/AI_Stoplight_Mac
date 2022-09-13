using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarFlowManager : MonoBehaviour
{
    public Object[] carModels;
    [Range(0,1)]
    public float[] flowIntesitivity;
    [Range(0,1)]
    public float[] turningProbability;

    //that is combined with the left turn, so by incrementing rightTurningProbability you are implicitly decreasing the left turning probability
    [Range(0,1)]
    public float[] rightTurningProbability; 
    private IntersectionColorManager intersection;

    private OccupiedSpawn[] spawnPoints;

    private Transform[] carsContainer;

    private List<List<SplineDone>> paths;
    private Transform[] stopPoints;

    private Transform[] destroyPoints;

    private Coroutine[] spawnCoroutine;


    private bool[] called;

    void Start()
    {
        int arrayLength = 0;
        foreach(Transform t in GetComponentInChildren<Transform>()){
            if (t.name.Contains("_Intersection")){
                arrayLength ++;
            }
        }
        intersection = GetComponent<IntersectionColorManager>();
        paths = new List<List<SplineDone>>();
        spawnPoints = new OccupiedSpawn[arrayLength];
        carsContainer = new Transform[arrayLength];
        stopPoints = new Transform[arrayLength];
        destroyPoints = new Transform[arrayLength];
        called = new bool[arrayLength];
        spawnCoroutine = new Coroutine[arrayLength];


        for (int i = 0; i<arrayLength; i++){
            called[i] = false;
            string directionName = (Coordinates)i + "_";
            

            spawnPoints[i] = GameObject.Find(directionName + "Spawn_Point" + this.name.Substring(12)).GetComponent<OccupiedSpawn>();
            carsContainer[i] = GameObject.Find(this.name+"/"+directionName + "Car_Container").GetComponent<Transform>();
            stopPoints[i] = GameObject.Find(this.name+"/"+directionName + "Stop_Point").GetComponent<Transform>();
            destroyPoints[i] = GameObject.Find(directionName + "Destroy_Point" + this.name.Substring(12)).GetComponent<Transform>();
            paths.Add(new List<SplineDone>(GameObject.Find(directionName + "Paths" + this.name.Substring(12) + "/").GetComponentsInChildren<SplineDone>()));
            
        }

        carModels = Resources.LoadAll<GameObject>("Prefab");


    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i< spawnPoints.Length; i++){
            if (flowIntesitivity[i] > 0.01f) {
                if (!called[i]) {
                    if (!spawnPoints[i].getSpawnOccupied()){
                        called[i] = true;
                        spawnCoroutine[i] = StartCoroutine(spawnCar(i));
                    }
                }
            }
        }
    }

    IEnumerator spawnCar(int i){
        
        yield return new WaitForSeconds(1f/flowIntesitivity[i]);

        var clone = Instantiate(carModels[Random.Range(0, carModels.Length)], spawnPoints[i].transform.position, Quaternion.Euler(0, -90,0), carsContainer[i]) as GameObject;
        clone.GetComponent<CarManager>().dir = (Coordinates)i;
        if (spawnPoints.Length == 4){
            clone.GetComponent<CarManager>().isTurn = Random.Range(0f,1f) <= turningProbability[i];
            if (clone.GetComponent<CarManager>().isTurn){
                if (Random.Range(0f, 1f) <= rightTurningProbability[i]) {
                    clone.GetComponent<SplineFollower>().setSpline(paths[i][0]); //0=right turn 
                }
                else{
                    clone.GetComponent<SplineFollower>().setSpline(paths[i][1]); //1=left turn
                }
            }
            else{
                clone.GetComponent<SplineFollower>().setSpline(paths[i][2]); //2=straght on             
            }
        }
        else if (spawnPoints.Length == 2){
            clone.GetComponent<CarManager>().isTurn = false;
            clone.GetComponent<SplineFollower>().setSpline(paths[i][0]);
        }
        clone.GetComponent<CarManager>().stopPoint = stopPoints[i].position;
        clone.GetComponent<CarManager>().oppositeCarsContainer = i % 2 == 0 ? carsContainer[i + 1] : carsContainer[i - 1];
        if (clone.GetComponent<CarManager>().isTurn) 
            clone.GetComponent<CarManager>().destroyPoint = destroyPoints[i>=2 ? (3-i) : (i+2) ].position;
        else
            clone.GetComponent<CarManager>().destroyPoint = destroyPoints[i].position;
        clone.GetComponent<CarManager>().intersection = intersection;
        clone.GetComponent<CarManager>().setMaxCaSpeed();
        clone.GetComponent<CarManager>().speed = clone.GetComponent<CarManager>().getMaxCarSpeed();
        yield return new WaitForSeconds(0.2f);
        called[i] = false;
    }

    public void setIntersection(IntersectionColorManager intersection){
        this.intersection = intersection;
    }

    public void resetSpawnPoint(){
        StopAllCoroutines();
        for (int i = 0; i<spawnPoints.Length; i++){ 
            called[i] = false;
            spawnPoints[i].resetIsOccupied();
            
        }
    }

    public Transform[] getCarsContainer(){
        return this.carsContainer;
    }

    public Transform getStopPoint(int index){
        return stopPoints[index];
    }


}
