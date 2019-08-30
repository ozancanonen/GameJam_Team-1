using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    public GameObject particleObject;
    public GameObject particleParentObject;
    private GameManager gm;

    private void Start()
    {
        gm = GetComponentInParent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player1" || col.tag == "Player2"/* || col.tag == "Construct"*/)
        { 
            GameObject particle = Instantiate(particleObject,transform.position,Quaternion.identity);
            particle.transform.parent = particleParentObject.transform;
            StartCoroutine(gm.DestroyThisAFter(particle, 1));
        }
            
    }
}
