using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Transform of the object that we want the camera to look at")]
    [SerializeField] Transform centerOfTheFollowedObject; // In this case its the body of spider   
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float damping;
    [SerializeField] float targetHeight = 30f;
    [SerializeField] float rotationSpeed = 130f;
    [SerializeField] float maxLookingAngle = 70f;
    [SerializeField] float minLookingAngle = -20f;

    Transform cameraLookTarget; // Object that is moved to 'centerOfTheFollowedObject', so that its rotation is not effeted by the FollowedObject  
    float zoom = 1;

    void Start()
    {
        cameraLookTarget = new GameObject().transform;
        cameraLookTarget.name = "_cameraLookTarget";
    }

    void FixedUpdate()
    {
        ZoomCamera();
        MoveCamera();
        RotateCamera();
    }

    void ZoomCamera()
    {  
        zoom += (-Input.mouseScrollDelta.y / 30);
        zoom = Mathf.Clamp(zoom, 0.5f, 1.2f);
    }

    void MoveCamera()
    {
        cameraLookTarget.position = centerOfTheFollowedObject.position + new Vector3(0, targetHeight * zoom, 0);

        Vector3 targetPosition = cameraLookTarget.position + cameraLookTarget.transform.forward * cameraOffset.z * zoom +
            cameraLookTarget.up * cameraOffset.y +
            cameraLookTarget.right * cameraOffset.x;

        //Debug.DrawLine(targetPosition, centerOfTheFollowedObject.position, Color.blue);

        HandleCameraCollision(centerOfTheFollowedObject.position, ref targetPosition);

        transform.position = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
        transform.LookAt(cameraLookTarget);
    }

    void RotateCamera()
    {
        // Rotation with mouse
        // Vector2 inputs = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        Vector2 inputs = RotationInputs();
        float x = cameraLookTarget.eulerAngles.x + inputs.x * rotationSpeed * Time.deltaTime;
        x = Mathf.Clamp(WrapAngle(x), minLookingAngle, maxLookingAngle);
        cameraLookTarget.rotation = Quaternion.Euler(x, cameraLookTarget.eulerAngles.y + inputs.y * rotationSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraCollision(Vector3 toTarget, ref Vector3 fromTarget)
    {
        RaycastHit hit;
        if (Physics.Linecast(toTarget, fromTarget, out hit))
        {
            Vector3 hitPoint = new Vector3(hit.point.x + hit.normal.x * .2f, hit.point.y, hit.point.z + hit.normal.z * .2f);
            fromTarget = new Vector3(hitPoint.x, fromTarget.y, hitPoint.z);
        }
    }

    float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    Vector2 RotationInputs()
    {
        Vector2 inputs = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
            inputs.y += 1;
        if (Input.GetKey(KeyCode.D))
            inputs.y -= 1;
        if (Input.GetKey(KeyCode.W))
            inputs.x += 1;
        if (Input.GetKey(KeyCode.S))
            inputs.x -= 1;

        return inputs;
    }
}
