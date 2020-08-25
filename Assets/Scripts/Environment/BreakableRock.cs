using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    public GameObject ScatteredObject;
    public ParticleSystem ParticleEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spider")
        {
            SwitchObjects();
            //AddParticleEffect();
            //AddSound();
        }
    }

    void SwitchObjects()
    {
        // Set original object off
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Set ScatteredObject on
        ScatteredObject.SetActive(true);

        //Invoke("FreezePieces", 15f);
    }

    void FreezePieces()
    {
        foreach (Transform rockPiece in ScatteredObject.transform)
        {
            Destroy(rockPiece.GetComponent<Rigidbody>());
            Destroy(rockPiece.GetComponent<MeshCollider>());
        }
    }

    /*
    void DestroyPiecesThatFellThroughTheGround()
    {
        foreach (Transform item in ScatteredObject.transform)
        {
            //Debug.Log(item.name + " / " + (transform.position.y - item.position.y));
            if ((transform.position.y - item.position.y) < -3f)
            {
                Debug.Log("Rock destroyed / " + (transform.position.y - item.position.y));
                Destroy(item.gameObject);
            }
                
        }
    }

    

    void AddParticleEffect()
    {
        //ParticleEffect.Play();
    }

    void AddSound()
    {

    }
    */
}
