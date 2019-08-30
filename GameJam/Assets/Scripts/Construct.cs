using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Rigidbody2D rigidB;
    public float speed;
    private bool move = true;
    private Vector3 direction;
    public GameObject collisionDirection;
    public GameObject bonfire;
    
    public void Move(Vector3 d, Quaternion rotation)
    {
        print("I am moving");
        direction = d;
        collisionDirection.transform.rotation = rotation;
        if(move)
            StartCoroutine(Travel());
    }
    IEnumerator Travel()
    {
        while(move)
        {
            gameObject.transform.position = new Vector3((transform.position.x + (direction.x * speed / 90)), (transform.position.y + (direction.y * speed / 90)), transform.position.z);
            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Destroyer":
            case "Wall":
            case "Construct":
            case "Player1":
            case "Player2":
                StopAllCoroutines();
                move = false;
                break;
            case "Player1Bullet":
            case "Player2Bullet":
            case "Bonfire":
                move = false;
                StopAllCoroutines();
                GameObject BF = Instantiate(bonfire, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, bonfire.transform.localPosition.z), bonfire.transform.rotation);
                Destroy(gameObject);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2" || col.gameObject.tag == "Destroyer")
            move = true;
    }

}
