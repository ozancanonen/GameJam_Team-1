using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public bool consoleController;
    public string enemyBulletTag;
    public string horizontalMovementInputButtons;
    public string verticalMovementInputButtons;
    public string fireMovementInputButtons;
    public string Skill1MovementInputButtons;
    public string Skill2MovementInputButtons;

    public GameObject prefabParentObject;
    public GameObject bullet;
    public GameObject lanternObject;
    public GameObject bulletParticles;
    public GameObject deadParticles;
    public GameObject wall;
    public GameObject wallBump;
    public GameObject playerHearth;
    public GameObject playerLanternHearth;
    public GameObject lanternDurabilityBarFill;
    public GameObject lanternDurabilityBar;


    private bool obstructed;
    private bool doneChanneling;
    public float channelingTime;
    private float channelingTimeCounter;
    public float knockback;

    public float moveSpeed;
    public float playerHealth;
    public float lanternDurabilityMagnifier;
    private bool canShoot;
    private bool nearAlter;
    private bool lanternOn=false;
    private bool inMeleeRange;
    private bool immobolized = false;
    public bool isDizzy=false;
    //private bool isAttacking = false;
    public Slider HealthSliderObject;
    public Slider lanternDurabilitySlider;
    private Vector2 movement;
    public BoxCollider2D melee;
    public Transform firingPos;
    public Transform bulletSpawnPos;
    public Transform particleSpawnPos;
    private GameObject prefabObject;
    private Rigidbody2D meleeInteraction;

    private SpriteRenderer sp;
    private Rigidbody2D rb;
    private Animator anim;
    private GameManager gm;

    private float lanternDurability = 100;
    private float waitTime;
    private bool doingInteraction;



    void Start()
    {
        lanternOn = lanternObject.activeSelf;
        canShoot = true;
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        sp = gameObject.GetComponent < SpriteRenderer>();
        gm = GetComponentInParent<GameManager>();
        if (!consoleController&&gameObject.tag=="Player1")
        {
            horizontalMovementInputButtons = "Horizontal1Keyboard";
            verticalMovementInputButtons= "Vertical1Keyboard";
        }
        if (!consoleController && gameObject.tag == "Player2")
        {
            horizontalMovementInputButtons = "Horizontal2Keyboard";
            verticalMovementInputButtons = "Vertical2Keyboard";
        }
    }

    void Update()
    {
        if (!immobolized)
        {
            if (Input.GetButtonDown(fireMovementInputButtons))
            {
                if (canShoot)
                {
                    Attack();
                    anim.SetFloat("attackState",1);
                }
            }
            if (Input.GetButtonDown(Skill1MovementInputButtons))
            {
                if (!obstructed)
                    StartCoroutine(buildConstruct(1f));
            }
            if (Input.GetButtonDown(Skill2MovementInputButtons))
            {
                Lantern();
            }
        }
        ProjectileRotationManager();
        GetCharacterInputs();
        Animate();
        if (nearAlter && lanternDurability < 100)
        {
            lanternDurability += lanternDurabilityMagnifier * Time.deltaTime;
            Debug.Log(lanternDurabilityBarFill.transform.localScale.x);
            lanternDurabilityBarFill.transform.localScale = new Vector3(lanternDurability/100, lanternDurabilityBarFill.transform.localScale.y, 1);
            if (lanternDurability >= 100)
            {
                lanternDurabilityBar.SetActive(false);
                playerLanternHearth.GetComponent<Animator>().SetTrigger("Unbreake");
                Lantern();
            }
        }
    }

    //we are using FixedUpdate for all physical related stuff 
    void FixedUpdate()
    {
        //we make the player move according to the player Input, we multiplied the value with time to make the movement at a constant speed 
        //not relative to fps
        if (!immobolized)
            rb.MovePosition(rb.position + movement.normalized * moveSpeed*Time.fixedDeltaTime);
    }

    //returns a value between -1 and 1 according to the pressed buttons that are defined for Horizontal in Unity(they are changeable)
    //we assign these values to "movement"
    void GetCharacterInputs()
    {
        movement.x = Input.GetAxisRaw(horizontalMovementInputButtons);
        movement.y = Input.GetAxisRaw(verticalMovementInputButtons);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Wall")
        {
            prefabObject = Instantiate(wallBump,col.contacts[0].point, firingPos.rotation*Quaternion.Euler(1,1,-90f));
            Destroy(prefabObject, 0.5f);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (gameObject.tag != col.gameObject.tag)
        {
            if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2" || col.gameObject.tag == "Construct")
            {
                inMeleeRange = true;
                meleeInteraction = col.attachedRigidbody;
            }
        }
        if (col.tag == "Altar")
        {
            nearAlter = true;
        }
        if (col.tag == "fireCollider")
        {
            TakeDamage(100);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Player1":
            case "Player2":
            case "Construct":
            case "Wall":
                obstructed = true;
                break;

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (gameObject.tag != col.gameObject.tag)
        {
            if (col.gameObject.tag == "Player1" || col.gameObject.tag == "Player2" || col.gameObject.tag == "Construct")
            {
                obstructed = false;
                inMeleeRange = false;
            }
        }
        if (col.tag == "Wall")
            obstructed = false;
        if (col.tag == "Altar")
        {
            Debug.Log("altar exit");
            nearAlter = false;
        }
    }
    public void TakeDamage(int damage)
    {
        if (lanternOn)
        {
            playerLanternHearth.GetComponent<Animator>().SetTrigger("Breake");
            Lantern();
            lanternObject.SetActive(false);
            lanternDurability = 0;
            lanternDurabilityBar.SetActive(true);
            lanternDurabilityBarFill.transform.localScale =new Vector3( 0f, lanternDurabilityBarFill.transform.localScale.y,1);
        }
        else
        {
            playerHealth -= damage;
            HealthSliderObject.value = playerHealth;
            if (playerHealth <= 0)
            {
                playerHearth.GetComponent<Animator>().SetTrigger("Breake");
                Dead();
            }
        }
    }

    void Lantern()
    {
        if (lanternOn)
        {
                lanternObject.SetActive(false);
                lanternOn = false;
        }
        else
        {
            if (lanternDurability >= 100)
            {
                gm.audioManager.Play("LanternIgnite");
                lanternObject.SetActive(true);
                lanternOn = true;
            }
        }
    }


    void ProjectileRotationManager()
    {
        //d press
        if (movement == new Vector2(1, 0))
            firingPos.rotation = Quaternion.Euler(0, 0, 0);
        //a press
        if (movement == new Vector2(-1, 0))
            firingPos.rotation = Quaternion.Euler(0, 0, 180);
        //w press
        if (movement == new Vector2(0, 1))
            firingPos.rotation = Quaternion.Euler(0, 0, 90);
        //s press
        if (movement == new Vector2(0, -1))
            firingPos.rotation = Quaternion.Euler(0, 0, 270);
        //d & w press
        if (movement == new Vector2(1, 1))
            firingPos.rotation = Quaternion.Euler(0, 0, 45);
        //d & s press
        if (movement == new Vector2(1, -1))
            firingPos.rotation = Quaternion.Euler(0, 0, 315);
        //a & w press
        if (movement == new Vector2(-1,1))
            firingPos.rotation = Quaternion.Euler(0, 0, 135);
        //a & s press
        if (movement == new Vector2(-1, -1))
            firingPos.rotation = Quaternion.Euler(0, 0, 225);
    }

    //we set the values we get from the player for the animator
    void Animate()
    {
        //To make the player look the way it last moved when stopped we don't make the movement Vector2 at he last frame to trigger
        //the right animation in the blend tree
        if (movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        } 
        anim.SetFloat("Speed", movement.magnitude);
    }

    private void Attack()
    {
        canShoot = false;
        waitTime = Time.fixedTime + 0.25f;
        channelingTimeCounter = channelingTime + Time.fixedTime;
        StartCoroutine(Melee());
        StartCoroutine(Channel());
    }
    IEnumerator Melee()
    {
        int i = 0;
        while (waitTime > Time.fixedTime && i < 2)
        {
            if (!Input.GetButton(fireMovementInputButtons))
            {
                i++;
                if (i < 2)
                {
                    StartCoroutine(attackStateManageWithDelay(0, 0.2f));
                    if (inMeleeRange)
                    {
                        Interaction();
                    }
                }
            }
            yield return null;
        }
    }
    void Firebolt()
    {
        Instantiate(bullet, bulletSpawnPos.position, firingPos.rotation);
        prefabObject = Instantiate(bulletParticles, particleSpawnPos.position, particleSpawnPos.rotation);
        prefabObject.transform.parent = prefabParentObject.transform;
        StartCoroutine(gm.DestroyThisAFter(prefabObject, 1));
        canShoot = true;
    }

    void Interaction ()
    {
        {
            
            switch (gameObject.tag)
            {
                case "Player1":
                    if (meleeInteraction.gameObject.tag == "Player2")
                    {
                        meleeInteraction.AddForce(ForceDirection() * knockback);
                        StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().Immobolize(1));
                        StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().DizzyFor(1));
                        meleeInteraction.gameObject.GetComponent<Player>().TakeDamage(0);
                        
                    }
                    if (meleeInteraction.gameObject.tag == "Construct")
                    {
                        moveConstruct();
                    }
                    break;
                case "Player2":
                    if (meleeInteraction.gameObject.tag == "Player1")
                    {
                        meleeInteraction.AddForce(ForceDirection() * knockback);
                        StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().Immobolize(1));
                        StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().DizzyFor(1));
                        meleeInteraction.gameObject.GetComponent<Player>().TakeDamage(0);

                    }
                    meleeInteraction.AddForce(ForceDirection() * knockback);
                    if (meleeInteraction.gameObject.tag == "Construct")
                    {
                        moveConstruct();
                    }
                    break;
            }
        }

    }

    private void moveConstruct()
    {
        Construct c = meleeInteraction.gameObject.GetComponent<Construct>();
        c.Move(ForceDirection(), firingPos.rotation);
    }
    //ForceDirection returns the Vecotor3 
    Vector3 ForceDirection()
    {
        float rot = firingPos.rotation.eulerAngles.z;
        Vector3 trans = new Vector3(0,0,0);
        switch (rot)
        {
            case 0:
            case 315:
            case 360:
                trans = transform.right;
                break;
            case 180:
            case 135:
                trans = -transform.right;
                break;
            case 90:
            case 45:
                trans = transform.up;
                break;
            case 270:
            case 225:
                trans = -transform.up;
                break;
        }
        return trans;
    }
    public void Dead()
    {
        immobolized = true;
        prefabObject =Instantiate(deadParticles, firingPos.position, firingPos.rotation);
        prefabObject.transform.parent = prefabParentObject.transform;
        anim.SetTrigger("Dead");
        StartCoroutine(gm.DestroyThisAFter(prefabObject, 1));
        StartCoroutine(gm.PlayerIsDeath(gameObject.tag));
    }

    IEnumerator attackStateManageWithDelay(float attackStateWillBe, float afterThisMuchTime)
    {
        yield return new WaitForSeconds(afterThisMuchTime);
        anim.SetFloat("attackState", attackStateWillBe);
    }

    IEnumerator buildConstruct(float waitingtime)
    {
        anim.SetFloat("attackState", 1);
        gm.audioManager.Play("Construct");
        StartCoroutine(Immobolize(waitingtime));
        yield return new WaitForSeconds(waitingtime);
        anim.SetFloat("attackState", 0);
        prefabObject = Instantiate(wall, bulletSpawnPos.position, wall.transform.rotation);
        prefabObject.GetComponent<Construct>().collisionDirection.transform.rotation = firingPos.rotation;
        prefabObject.transform.parent = prefabParentObject.transform;
    }
    IEnumerator Immobolize(float channeling)
    {
        immobolized = true;
        yield return new WaitForSeconds(channeling);
        immobolized = false;
    }
    IEnumerator Channel()
    {
        while (Input.GetButton(fireMovementInputButtons))
        {
            immobolized = true;
            if (channelingTimeCounter < Time.fixedTime)
            {
                StartCoroutine(attackStateManageWithDelay(0, 0));
                Firebolt();
                canShoot = true;
                channelingTimeCounter = channelingTime + Time.fixedTime;
            }
            yield return null;
        }
        canShoot = true;
        immobolized = false;
    }

    IEnumerator DizzyFor(float time)
    {
        anim.SetBool("Dizzy", true);
        yield return new WaitForSeconds(time);
        anim.SetBool("Dizzy", false);

    }

}
