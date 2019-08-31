using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int player;
    public float bulletSpeed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
       
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Player":
                if (player != col.gameObject.GetComponent<Player>().player)
                {
                    col.gameObject.GetComponent<Player>().TakeDamage(100);
                    Destroy(gameObject);
                }
                break;
            case "Construct":
                Destroy(gameObject);
                break;
        }
    }


}
