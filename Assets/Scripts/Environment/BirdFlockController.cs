using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFlockController : MonoBehaviour
{
    [Range(1, 200)] [SerializeField] int birdsInFlock = 1;
    [Range(1, 20)] [SerializeField] int numberOfFlocks = 1;
    [Range(1, 300)] [SerializeField] float flocksAreaDiameter = 100f;
    [Range(1, 100)] [SerializeField] float flocksAreaHeight = 100f;    
    [SerializeField] GameObject birdPrefab;
    Transform[] flockTargets;

    List<Bird> birds;

    public List<Transform> restingPlaces;

    void Start()
    {
        CreateTargets();
        CreateBirds();
        MoveFlockTarget(0);
        MoveFlockTarget(1);
    }
    
    void CreateTargets()
    {
        flockTargets = new Transform[numberOfFlocks];

        for (int i = 0; i < numberOfFlocks; i++)
        {
            GameObject t = new GameObject();
            t.name = "Target" + i;
            t.transform.parent = transform;
            flockTargets[i] = t.transform;
        }
    }

    void CreateBirds()
    {
        birds = new List<Bird>();

        for (int i = 0; i < birdsInFlock; i++)
        {
            float posX = Random.Range(-flocksAreaDiameter, flocksAreaDiameter);
            float posY = Random.Range(-flocksAreaHeight, flocksAreaHeight);
            float posZ = Random.Range(-flocksAreaDiameter, flocksAreaDiameter);
            Vector3 spawnPosition = new Vector3(posX, posY, posZ);

            GameObject go = Instantiate(birdPrefab, transform.position + spawnPosition, Quaternion.identity, this.transform);
            int targetIndex = Random.Range(0, flockTargets.Length);
            go.GetComponent<Bird>().Init(this, flockTargets, targetIndex);
            birds.Add(go.GetComponent<Bird>());
        }
    }
    

    public void MoveFlockTarget(int targetIndex)
    {
        //Debug.Log("MoveFlockTarget");

        float posX = Random.Range(-flocksAreaDiameter, flocksAreaDiameter);
        float posY = Random.Range(-flocksAreaHeight, flocksAreaHeight);
        float posZ = Random.Range(-flocksAreaDiameter, flocksAreaDiameter);
        Vector3 nextPosition = new Vector3(posX, posY, posZ);
        flockTargets[targetIndex].localPosition = nextPosition;

        MixFlocks();
        Rest();
    }

    void MixFlocks()
    {
        int random = Random.Range(0, 3);

        for (int i = 0; i < random; i++)
        {
            int birdIndex = Random.Range(0, birdsInFlock);
            birds[birdIndex].SwitchTarget();
        }
    }

    void Rest()
    {
        int random = Random.Range(0, 2);

        for (int i = 0; i < random; i++)
        {
            int birdIndex = Random.Range(0, birdsInFlock);
            birds[birdIndex].Rest();
        }
    }

    void OnDrawGizmos()
    {
        // Flock Boundry
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(flocksAreaDiameter * 2, flocksAreaHeight * 2, flocksAreaDiameter * 2));

        if (flockTargets == null)
            return;

        // Flock Target
        Gizmos.color = Color.green;
        for (int i = 0; i < flockTargets.Length; i++)
        {
            Gizmos.DrawWireSphere(flockTargets[i].position, 20);
        }     
    }
}
