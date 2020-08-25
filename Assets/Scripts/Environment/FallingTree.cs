using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTree : MonoBehaviour
{
    [SerializeField] float xAngle = 80f;
    [SerializeField] float fallSpeed = 5f;

    float speed;

    bool fallen = false;

    void Start()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Spider" || fallen)
            return;

        fallen = true;

        Debug.Log(collision.transform.GetComponent<Rigidbody>().velocity.magnitude);
        speed = collision.transform.GetComponent<Rigidbody>().velocity.magnitude;
        Vector3 forceDirection = transform.position - collision.transform.position;
        forceDirection.Normalize();
        Vector3 horizontalDirection = new Vector3(forceDirection.x, 0, forceDirection.z);
        transform.rotation = Quaternion.LookRotation(horizontalDirection, Vector3.up);

        StartCoroutine(RotateTree());
    }

    IEnumerator RotateTree()
    {
        float addedDegress = 0;
        bool done = false;
        float step = 0;
        while (!done)
        {
            step += speed /* fallspeed */ * Time.deltaTime;
            transform.Rotate(new Vector3(step, 0, 0));
            addedDegress += step;
            if (addedDegress >= xAngle)
                done = true;

            yield return null;
        }

        FreezeTree();
    }

    void FreezeTree()
    {
        Destroy(GetComponent<Collider>());
        Destroy(this);
    }
}
