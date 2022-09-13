using UnityEngine;

public class OccupiedSpawn : MonoBehaviour
{
    private bool spawnOccupied;
    private Collider other;
    // Start is called before the first frame update
    void Start()
    {
        spawnOccupied = false;
        other = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other){
        if (other.transform.name.Contains("base"))
            spawnOccupied = true;
            this.other = other;
    }

    void OnTriggerExit(Collider other){
        if (other.transform.name.Contains("base"))
            spawnOccupied = false;
        
    }

    public bool getSpawnOccupied(){
        if (other == null || !other.gameObject.scene.IsValid())
            spawnOccupied = false;
        return spawnOccupied;
    }

    public void resetIsOccupied(){
        spawnOccupied = false;
    }

}
