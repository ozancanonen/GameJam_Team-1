using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public float lifetime = 6;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Player1":
            case "Player2":
                col.gameObject.GetComponent<Player>().Dead();
                break;
        }
    }
}
