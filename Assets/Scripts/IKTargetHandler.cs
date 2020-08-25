using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTargetHandler : MonoBehaviour
{
    [SerializeField] Transform[] raycasters;
    [SerializeField] Transform[] legs;
    [SerializeField] float stepTreshold = 13.7f;
    [SerializeField] float stepSpeed = 0.8f;    
    [SerializeField] float stepHeight = 10.1f;

    SpiderController controller;
    Transform[] targets;
    IKFabric[] legFabrics;  
    bool[] moving;
    Vector3 legFix = new Vector3(0, 0f, 0);
    int[][] movingPairs;

    int activePair = 0;
    int numberOfLegsLooped = 0;
    float stepSpeedMultiplier = 1f;



    //footprint
    public GameObject footprintPrefab;

    public Transform[] GetTargets()
    {
        return targets;
    }

    public void SetStepSpeedMultiplier(float multiplier)
    {
        stepSpeedMultiplier = multiplier;
    }

    /*
    IEnumerator RandomLegAdjustment()
    {
        while (StandingStill)
        {
            float timer = Random.Range(1f, 2f);

            yield return new WaitForSeconds(timer);

            int randomLeg = Random.Range(0, 7);
            //Debug.Log(randomLeg);

            RaycastHit hit;
            if (Physics.Raycast(raycasters[randomLeg].position, -raycasters[randomLeg].up, out hit))
            {
                if (!moving[randomLeg])
                {
                    if (canLegMove(randomLeg))
                        StartCoroutine(MoveLeg(targets[randomLeg], targets[randomLeg].position, hit.point + legFix, randomLeg));
                }
            }
                
        }    
    }
    */

    void Start()
    {
        controller = FindObjectOfType<SpiderController>().GetComponent<SpiderController>();


        // Get each legFabric from legs
        legFabrics = new IKFabric[legs.Length];
        for (int i = 0; i < legFabrics.Length; i++)
        {
            legFabrics[i] = legs[i].GetComponentInChildren<IKFabric>();
            //Debug.Log(legFabrics[i]);
        }


        // Get targets from each legFabric
        targets = new Transform[legs.Length];
        for (int i = 0; i < legs.Length; i++)
        {
            targets[i] = legFabrics[i].Target;
        }
        
        moving = new bool[targets.Length];



        int[] list0 = new int[4] { 0, 3, 4, 7 };
        int[] list1 = new int[4] { 1, 2, 5, 6 };

        movingPairs = new int[][] { list0, list1 };

        //StartCoroutine(RandomLegAdjustment());
        //CreateSoundSystem();
    }

    void FixedUpdate()
    {
        WalkingManager();
    }

    int getLegsMovingPairs(int legIndex)
    {
        foreach (int item in movingPairs[0])
        {
            if (item == legIndex)
                return 0;
        }

        return 1;
    }

    void WalkingManager()
    {
        // ryhmät vuorotellen
        for (int i = 0; i < movingPairs.Length; i++)    // tulee 0 tai 1
        {
            foreach (int legIndex in movingPairs[i])
            {
                // Move only activePairs legs
                if(i == activePair && canLegMove(i))
                {
                    numberOfLegsLooped++;

                    if (!moving[legIndex])
                    {
                        int legGroup = i;
                        RaycastHit hit;

                        if (Physics.Raycast(raycasters[legIndex].position, -raycasters[legIndex].up, out hit))
                        {
                            Debug.DrawLine(raycasters[legIndex].position, hit.point, Color.magenta);
                            if (Vector3.Distance(hit.point, targets[legIndex].position) > stepTreshold)
                            {
                                if (legFabrics[legIndex].CompleteLength >= Vector3.Distance(legs[legIndex].position, hit.point))
                                {
                                    //Debug.Log(hit.normal);
                                    //Debug.Log(Quaternion.FromToRotation(Vector3.up, hit.normal));
                                    

                                    StartCoroutine(MoveLeg(targets[legIndex], targets[legIndex].position, hit.point + legFix, legIndex, hit.normal));
                                }
                                else
                                {
                                    AnotherRaycastTry(legIndex, hit, legGroup);
                                }
                            }
                        }
                        else
                        {
                            AnotherRaycastTry(legIndex, hit, legGroup);
                        }
                    }

                    if (numberOfLegsLooped == movingPairs[i].Length)
                    {
                        if (activePair == 0)
                            activePair = 1;
                        else
                            activePair = 0;

                        numberOfLegsLooped = 0;
                    }
                        
                }
            }
        }
    }

    /*
    void LookForNewTargetPosition()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            int legGroup = getLegsMovingPairs(i);
            RaycastHit hit;

            if (Physics.Raycast(raycasters[i].position, -raycasters[i].up, out hit))
            {
                Debug.DrawLine(raycasters[i].position, hit.point, Color.magenta);
                if (Vector3.Distance(hit.point, targets[i].position) > stepTreshold)
                {
                    if (legFabrics[i].CompleteLength >= Vector3.Distance(legs[i].position, hit.point))
                    {
                        if (!moving[i])
                        {
                            if (canLegMove(legGroup))
                                StartCoroutine(MoveLeg(targets[i], targets[i].position, hit.point + legFix, i));
                        }
                    }
                    else
                    {
                        // Raycast again with modified angle           
                        Vector3 modifiedAngle;
                        switch (i)
                        {                                
                            case 0:
                                modifiedAngle = new Vector3(-15, 0, 0);
                                break;
                            case 1:
                                modifiedAngle = new Vector3(-15, 0, 0);
                                break;
                            case 2:
                                modifiedAngle = new Vector3(0, 0, 15);
                                break;
                            case 3:
                                modifiedAngle = new Vector3(0, 0, -15);
                                break;
                            case 4:
                                modifiedAngle = new Vector3(0, 0, 15);
                                break;
                            case 5:
                                modifiedAngle = new Vector3(0, 0, -15);
                                break;
                            case 6:
                                modifiedAngle = new Vector3(15, 0, 0);
                                break;
                            case 7:
                                modifiedAngle = new Vector3(15, 0, 0);
                                break;
                            default:
                                modifiedAngle = Vector3.zero;
                                break;
                        }

                        if (Physics.Raycast(raycasters[i].position, -raycasters[i].up + modifiedAngle, out hit))
                        {
                            if (legFabrics[i].CompleteLength >= Vector3.Distance(legs[i].position, hit.point))
                            {
                                Debug.Log("Found a new leg position: " + targets[i].name);

                                if (!moving[i])
                                {
                                    if (canLegMove(legGroup))
                                        StartCoroutine(MoveLeg(targets[i], targets[i].position, hit.point + legFix, i));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    */

    void AnotherRaycastTry(int i, RaycastHit hit, int legGroup)
    {
        // Raycast again with modified angle           
        Vector3 modifiedAngle;
        switch (i)
        {
            case 0:
                modifiedAngle = new Vector3(-15, 0, 0);
                break;
            case 1:
                modifiedAngle = new Vector3(-15, 0, 0);
                break;
            case 2:
                modifiedAngle = new Vector3(0, 0, 15);
                break;
            case 3:
                modifiedAngle = new Vector3(0, 0, -15);
                break;
            case 4:
                modifiedAngle = new Vector3(0, 0, 15);
                break;
            case 5:
                modifiedAngle = new Vector3(0, 0, -15);
                break;
            case 6:
                modifiedAngle = new Vector3(15, 0, 0);
                break;
            case 7:
                modifiedAngle = new Vector3(15, 0, 0);
                break;
            default:
                modifiedAngle = Vector3.zero;
                break;
        }

        if (Physics.Raycast(raycasters[i].position, -raycasters[i].up + modifiedAngle, out hit))
        {
            if (legFabrics[i].CompleteLength >= Vector3.Distance(legs[i].position, hit.point))
            {
                Debug.Log("Found a new leg position: " + targets[i].name);

                Debug.DrawLine(raycasters[i].position, hit.point, Color.blue);
                if (Vector3.Distance(hit.point, targets[i].position) > stepTreshold)
                {
                    if (legFabrics[i].CompleteLength >= Vector3.Distance(legs[i].position, hit.point))
                    {
                        StartCoroutine(MoveLeg(targets[i], targets[i].position, hit.point + legFix, i, hit.normal));
                    }
                }
            }
        }
    }

    bool canLegMove(int legGroup)
    {
        for (int j = 0; j < movingPairs.Length; j++)
        {
            if (j != legGroup)
            {
                foreach (int legIndex in movingPairs[j])
                {
                    if (moving[legIndex])
                        return false;
                }
            }
        }

        return true;
    }

    IEnumerator MoveLeg(Transform target, Vector3 startPosition, Vector3 endPosition, int index, Vector3 endRotation)
    {
        moving[index] = true;

        float val = 0f;
        Vector3 startPos = startPosition;
        Vector3 targetPos = endPosition;
        targetPos.y += stepHeight;
        bool halfWay = false;

        while(Vector3.Distance(target.position, endPosition) > 0.01f)
        {
            if(val >= 0.5f && !halfWay)
            {
                startPos.y += stepHeight;
                targetPos.y -= stepHeight;
                halfWay = true;
            }
                
            target.position = Vector3.Lerp(startPos, targetPos, val);
            val += stepSpeed * stepSpeedMultiplier * controller.speedMultiplier * Time.deltaTime;
            yield return null;
        }

        moving[index] = false;
        CreateFootprint(endPosition, endRotation);
    }

    void CreateFootprint(Vector3 position, Vector3 rotation)
    {
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, rotation);

        // Quaternion.identity pitää korvata raycastin normaalilla
        GameObject footprint = Instantiate(footprintPrefab, position + new Vector3(0, 0.0005f, 0), Quaternion.FromToRotation(Vector3.up, rotation));
    }
}
