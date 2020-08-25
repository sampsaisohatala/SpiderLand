using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Range(1, 10)]
    public float speedMultiplier = 1;

    [SerializeField] float movementSpeed = 6;
    [SerializeField] float rotationSpeed = 6;

    [SerializeField] float bodyDistanceFromGround = 11.5f;
    [SerializeField] float bodyMovementDamp = 8f;
    [SerializeField] float bodyRotationDamp = 6f;
    [SerializeField] float bodyRotationTreshold = .5f;

    IKTargetHandler ikHandler;
    Transform[] ikTargets;
    Transform mesh;
    Vector3 target;

    [SerializeField] float turningStepSpeedMultiplayer = 2f;

    float meshLength;

    void Start()
    {
        ikHandler = GetComponent<IKTargetHandler>();
        ikTargets = ikHandler.GetTargets();
        mesh = transform.Find("Mesh");
        meshLength = mesh.Find("Body").GetComponent<CapsuleCollider>().height * transform.localScale.x;
    }

    void FixedUpdate()
    {
        CheckForNewPositionToMove();

        if (target == Vector3.zero)
            return;

        MoveToTargetWithClick();
        
        CalculatePosition();
        CalculateRotation();
    }

    void CheckForNewPositionToMove()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                target = hit.point;
        }
    }

    void CalculatePosition()
    {
        float yy = CalculateYPosition();
        Vector3 newPos = new Vector3(mesh.transform.position.x, yy, mesh.transform.position.z);
        mesh.transform.position = Vector3.Lerp(mesh.transform.position, newPos, bodyMovementDamp * Time.deltaTime);
    }

    float CalculateYPosition()
    {
        float highestPoint = 0f;
        RaycastHit hit;

        for (int i = -1; i <= 1; i++)
        {
            if (Physics.Raycast(mesh.position + (i * ((meshLength / 2) * .65f) * mesh.forward) + new Vector3(0, 1.5f, 0), -Vector3.up, out hit))
            {
                //Debug.DrawRay(mesh.position + (i * ((meshLength / 2) * .65f) * mesh.forward) + new Vector3(0, 1.5f, 0), -Vector3.up, Color.cyan);

                if (hit.point.y > highestPoint)
                    highestPoint = hit.point.y;
            }
        }

        return highestPoint + bodyDistanceFromGround;
    }

    void CalculateRotation()
    {
        Vector3 averageNormal = Vector3.zero;
        Vector3[] raycastHitpoits = new Vector3[3];
        int succesfulRays = 0;
        RaycastHit hit;

        for (int i = -1; i <= 1; i++)
        {
            if (Physics.Raycast(mesh.position + (i * ((meshLength / 2) * .65f) * mesh.forward), -Vector3.up, out hit))
            {
                averageNormal += hit.normal;
                succesfulRays++;
                raycastHitpoits[i + 1] = hit.point;
            }
            else
                raycastHitpoits[i + 1] = Vector3.zero;
        }

        averageNormal /= succesfulRays;

        if(succesfulRays > 0)
        {
            Quaternion targetRot = Quaternion.FromToRotation(mesh.up, averageNormal) * mesh.rotation;
            mesh.rotation = Quaternion.Lerp(mesh.rotation, targetRot, bodyRotationDamp * Time.deltaTime);
            Quaternion localRot = mesh.localRotation;
            localRot.y = 0;
            mesh.localRotation = localRot;
        }
    }


    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(averagePosition, .02f);
    }
    */

    #region debug moving

    void MoveToTargetWithClick()
    {
        Vector3 targetDirection = target - transform.position;
        float singleStep = (rotationSpeed / 30) * speedMultiplier * Time.deltaTime;
        
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        newDirection.y = 0f;
        transform.rotation = Quaternion.LookRotation(newDirection);

        float t = (targetDirection != Vector3.zero) ? Quaternion.LookRotation(targetDirection).y : 0f;
        float tt = transform.rotation.y;

        // Add legSpeed when turning, so legs can keep up
        if (Mathf.Abs(t - tt) > .1f)
            ikHandler.SetStepSpeedMultiplier(turningStepSpeedMultiplayer);
        else
            ikHandler.SetStepSpeedMultiplier(1);
            /*
            if (Mathf.Abs(t - tt) > .1f)
                ikHandler.stepSpeed = originalStepSpeed * turningStepSpeedMultiplayer;
            else
                ikHandler.stepSpeed = originalStepSpeed;
            */

        if (Mathf.Abs(t - tt) < bodyRotationTreshold)
        MoveWithClick();
    }

    void MoveWithClick()
    {
        float step = movementSpeed * speedMultiplier * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) < .1f)
            target = Vector3.zero;
     
    }
    #endregion
}
