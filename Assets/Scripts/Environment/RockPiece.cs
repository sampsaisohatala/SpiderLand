using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPiece : MonoBehaviour
{
    Rigidbody _rigidbody;
    bool hasWaited = false;


    void Start()
    {    
        _rigidbody = GetComponent<Rigidbody>();
        Invoke("Waited", 5f);
    }

    void Update()
    {
        if (!hasWaited)
            return;

        if (_rigidbody.velocity.magnitude == 0)
            Freeze();

        // If rockpiece is falling through ground will trigger this
        if (_rigidbody.velocity.magnitude > 100)
            Destroy();
    }

    void Waited()
    {
        hasWaited = true;
    }

    void Freeze()
    {
        //Debug.Log("RockPiece freeze");
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<MeshCollider>());
        Destroy(this);
    }


    void Destroy()
    {
        //Debug.Log("RockPiece Destroy");
        Destroy(gameObject);
    }
}
