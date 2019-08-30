using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject endOfLevelMenuUI;
    public GameObject Player1HasWonText;
    public GameObject Player2HasWonText;
    public GameObject fireCollider;
    public GameObject flameParticle;
    public GameObject layer2Light;
    public GameObject particleParentObject;
    public GameObject eyesObject;
    public AudioManager audioManager;
    private Vector2 screenSize = new Vector2(Screen.width, Screen.height);
    private float timeCounter;
    public float timeBetweenEyesSpawn;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        timeCounter += Time.deltaTime;
        if(timeCounter> timeBetweenEyesSpawn)
        {
            spawnEyes();
            timeCounter = 0;
        }

    }
    private void spawnEyes()
    {
        float x = Random.Range(-10,10);//if scene sizew changes gotta change as well
        float y = Random.Range(-10, 10);
        GameObject eyes = Instantiate(eyesObject, new Vector3(x,y), Quaternion.identity);
        Destroy(eyes,1);
    }


    public IEnumerator footStepSounds(string sound1, string sound2, float waitingTime)
    {
        string[] currentSound = { sound1 , sound2};

        for (int i = 0; i <2; i++)
        {
            audioManager.Play(currentSound[i]);
            yield return new WaitForSeconds(waitingTime);
        }
    }



    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        StartCoroutine(footStepSounds("Wood1", "Wood1", 0.4f));
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public IEnumerator PlayerIsDeath(string playerTag)
    {
        yield return new WaitForSeconds(2f);
        if (playerTag == "Player1")
        {
            Player2HasWonText.SetActive(true);
        }
        if (playerTag == "Player2")
        {
            Player1HasWonText.SetActive(true);
        }
        Time.timeScale = 0;
        endOfLevelMenuUI.SetActive(true);
    }

    public IEnumerator DestroyThisAFter(GameObject thisObject, float destroyAfter)
    {
        yield return new WaitForSeconds(destroyAfter);
        Destroy(thisObject);
    }

}
