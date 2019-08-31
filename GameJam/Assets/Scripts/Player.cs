using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private string[] horizontalMovementControls = new string[] { "Horizontal1Keyboard", "Horizontal2Keyboard" };
    private string[] verticalMovementControls = new string[] { "Vertical1Keyboard", "Vertical2Keyboard" };
    private string[] fireControls = new string[] { "FirePlayer1", "FirePlayer2" };
    private string[] skill1Controls = new string[] { "Skill1Player1", "Skill1Player2" };
    private string[] skill2Controls = new string[] { "Skill2Player1", "Skill2Player2" };

    public int player;
    public float playerHealth;
    public float moveSpeed;
    public float knockback;
    public float channelingTime;
    public float timeBetweenEyes;
    public float lanternDurabilityMagnifier;
    private float channelingTimeCounter;
    private float lanternDurability = 100;
    private float waitTime;
    private int eyeCounter;

    public GameObject eyes;
    public GameObject prefabParentObject;
    public GameObject bullet;
    public GameObject bulletParticles;
    public GameObject deadParticles;
    public GameObject wall;
    public GameObject wallBump;
    public GameObject playerHearth;
    public GameObject playerLanternHearth;
    public GameObject lanternDurabilityBarFill;
    public GameObject lanternDurabilityBar;
    private GameObject lanterLight;
    private GameObject lanternObject;
    private GameObject prefabObject;
    public Slider HealthSliderObject;
    public Slider lanternDurabilitySlider;
    public BoxCollider2D melee;
    public Transform firingPos;
    public Transform bulletSpawnPos;
    public Transform particleSpawnPos;
    private SpriteRenderer sp;
    private Rigidbody2D meleeInteraction;
    private Rigidbody2D rb;
    private Animator anim;
    private GameManager gm;

    private bool obstructed;
    private bool doneChanneling;
    private bool canShoot;
    private bool nearAlter;
    private bool lanternOn=false;
    private bool inMeleeRange;
    private bool immobolized = false;
    private bool doingInteraction;
    public bool isDizzy=false;
    //public bool consoleController;
    private Vector2 movement;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        sp = gameObject.GetComponent < SpriteRenderer>();
        gm = GetComponentInParent<GameManager>();
        lanterLight = transform.Find("LanternLight").gameObject;
        lanternObject = transform.Find("FiringPoint/Lantern").gameObject;
    }

    void Start()
    {
        StartCoroutine(StartGame());
        player = player - 1;
        lanternOn = lanternObject.activeSelf;
        canShoot = true;
    }
    void Update()
    {
        if (!immobolized)
        {
            if (Input.GetButtonDown(fireControls[player]))
            {
                if (canShoot)
                {
                    Attack();
                    anim.SetFloat("attackState",1);
                }
            }
            if (Input.GetButtonDown(skill1Controls[player]))
            {
                if (!obstructed)
                    StartCoroutine(buildConstruct(1f));
            }
            if (Input.GetButtonDown(skill2Controls[player]))
            {
                Lantern();
            }
        }
        ProjectileRotationManager();
        GetCharacterInputs();
        Animate();

        if (movement.x == 0 && movement.y == 0 && lanternDurability < 100 && nearAlter)
        {
            lanternDurability += lanternDurabilityMagnifier * Time.deltaTime;
            Debug.Log(lanternDurabilityBarFill.transform.localScale.x);
            lanternDurabilityBarFill.transform.localScale = new Vector3(lanternDurability / 100, lanternDurabilityBarFill.transform.localScale.y, 1);
            if (lanternDurability >= 100)
            {
                lanternDurabilityBar.SetActive(false);
                playerLanternHearth.GetComponent<Animator>().SetTrigger("Unbreake");
                //Lantern();
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
        movement.x = Input.GetAxisRaw(horizontalMovementControls[player]);
        movement.y = Input.GetAxisRaw(verticalMovementControls[player]);
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

        switch (col.gameObject.tag)
        {
            case "Player":
                if (col.gameObject.GetComponent<Player>().player != player)
                {
                    goto case "Construct";
                }
                break;
            case "Construct":
                obstructed = true;
                meleeInteraction = col.attachedRigidbody;
                inMeleeRange = true;
                break;
            case "Wall":
                //gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
                obstructed = true;
                break;
            case "Altar":
                nearAlter = true;
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Player":
            case "Construct":
            case "Wall":
                obstructed = true;
                break;

        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Player":
                if (col.gameObject.GetComponent<Player>().player != player)
                {
                    goto case "Construct";
                }
                break;
            case "Construct":
                obstructed = false;
                inMeleeRange = false;
                break;
            case "Wall":
                obstructed = false;
                break;
            case "Altar":
                nearAlter = false;
                break;
        }   
    }
    public void TakeDamage(int damage)
    {
        if (lanternOn)
        {
            playerLanternHearth.GetComponent<Animator>().SetTrigger("Breake");
            Lantern();
            lanterLight.SetActive(false);
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
            StartCoroutine(spawnEyes());
            lanterLight.SetActive(false);
            lanternObject.SetActive(false);
            lanternOn = false;
        }
        else
        {
            if (lanternDurability >= 100)
            {
                StopCoroutine(spawnEyes());
                gm.audioManager.Play("LanternIgnite");
                lanterLight.SetActive(true);
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
            if (!Input.GetButton(fireControls[player]))
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
        bullet.GetComponent<Bullet>().player = player;
        Instantiate(bullet, bulletSpawnPos.position, firingPos.rotation);
        prefabObject = Instantiate(bulletParticles, particleSpawnPos.position, particleSpawnPos.rotation);
        prefabObject.transform.parent = prefabParentObject.transform;
        StartCoroutine(gm.DestroyThisAFter(prefabObject, 1));
        canShoot = true;
    }

    void Interaction ()
    {
        {
            switch (meleeInteraction.gameObject.tag)
            {
                case "Player":
                    if (meleeInteraction.gameObject.GetComponent<Player>().player != player)
                    {
                    meleeInteraction.AddForce(ForceDirection() * knockback);
                    StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().Immobolize(1));
                    StartCoroutine(meleeInteraction.gameObject.GetComponent<Player>().DizzyFor(1));
                    meleeInteraction.gameObject.GetComponent<Player>().TakeDamage(0);
                    }
                    meleeInteraction.AddForce(ForceDirection() * knockback);
                    break; 
                case "Construct":
                    moveConstruct();
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
        StartCoroutine(gm.PlayerIsDeath(player));
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
        while (Input.GetButton(fireControls[player]))
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
    IEnumerator StartGame()
    {
        StartCoroutine(Immobolize(4));
        yield return new WaitForSeconds(3);
        Lantern();
    }
    IEnumerator spawnEyes()
    {
        eyeCounter++;
        do
        {
            if (eyeCounter > 1)
            {
                eyeCounter--;
                break;
            }
            GameObject tempEyes = Instantiate(eyes, transform.position, eyes.transform.rotation);
            Destroy(tempEyes, 1);
            yield return new WaitForSeconds(timeBetweenEyes);
        } while (!lanternOn);
    }
}
