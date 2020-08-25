using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] float speedMultiplier = 1;

    BirdFlockController controller;
    Transform[] targets;

    [SerializeField] float flySpeed = 8f;
    [SerializeField] float flySpeedVariation = 2f;
    [SerializeField] float turnSpeed = 7f;
    [SerializeField] float turnSpeedVariation = 2f;
    [SerializeField] float randomOffset = 12f;


    int targetIndex;
    Vector3 _randomOffset;
    Vector3 targetPosition;

    public void Init(BirdFlockController birdFlockController, Transform[] flockTarget, int index)
    {
        controller = birdFlockController;
        targets = flockTarget;
        targetIndex = index;

        _randomOffset = new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset));
        flySpeed += Random.Range(-flySpeedVariation, flySpeedVariation);
        turnSpeed += Random.Range(-turnSpeedVariation, turnSpeedVariation);
    }

    public void SwitchTarget()
    {
        int newTargetIndex = targetIndex;
        while(newTargetIndex == targetIndex)
            newTargetIndex = Random.Range(0, targets.Length);

        targetIndex = newTargetIndex;
    }

    public void Rest()
    {

    }

    void Update()
    {
        Fly();
    }

    void Fly()
    {
        Vector3 targetDirection = (targets[targetIndex].position + _randomOffset) - transform.position;
        float singleStep = (turnSpeed / 30) * speedMultiplier * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        float t = (targetDirection != Vector3.zero) ? Quaternion.LookRotation(targetDirection).y : 0f;
        float tt = transform.rotation.y;

        Debug.DrawLine(transform.position, (targets[targetIndex].position + _randomOffset), Color.magenta);



        float step = flySpeed * speedMultiplier * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        transform.position += transform.forward * step;


        if (Vector3.Distance(transform.position, (targets[targetIndex].position + _randomOffset)) < .1f)
            controller.MoveFlockTarget(targetIndex);
    }
}
