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
    private GameManager gm;

    private void Start()
    {
        gm = gameObject.GetComponentInParent<GameManager>();
    }
    public void Move(Vector3 d, Quaternion rotation)
    {
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
            case "Player":
                StopAllCoroutines();
                move = false;
                break;
            case "Bullet":
            case "Bonfire":
                move = false;
                StopAllCoroutines();
                GameObject BF = Instantiate(bonfire, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, bonfire.transform.localPosition.z), bonfire.transform.rotation);
                gm.audioManager.Play("FireSound");
                StartCoroutine(gm.PauseAfter("FireSound",5f));
                gameObject.SetActive(false);
                //Destroy(gameObject,7f);
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "Player" || col.gameObject.tag == "Destroyer")
            move = true;
    }

}
