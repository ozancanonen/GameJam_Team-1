using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

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
        if (gameObject.tag == "Player1Bullet")
            if(col.gameObject.tag == "Player2")
            {
            col.gameObject.GetComponent<Player>().TakeDamage(100);
            Destroy(gameObject);
            }
        if (gameObject.tag == "Player2Bullet")
            if (col.gameObject.tag == "Player1")
            {
                col.gameObject.GetComponent<Player>().TakeDamage(100);
                Destroy(gameObject);
            }
        if (col.tag == "Construct")
            Destroy(gameObject);

    }


}
