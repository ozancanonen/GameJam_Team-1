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
    public GameObject[] eyesObject;
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
        int i = Random.Range(0,2);
        float x = Random.Range(-20,20);//if scene sizew changes gotta change as well
        float y = Random.Range(-20, 20);

        GameObject eyes = Instantiate(eyesObject[i], new Vector3(x,y,-0.5f), Quaternion.identity);
        Destroy(eyes,0.75f);
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
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public IEnumerator PauseAfter(string name, float time)
    {
        yield return new WaitForSeconds(time);
        audioManager.Pause(name);

    }

    public IEnumerator PlayerIsDeath(int player)
    {
        player = player + 1;
        yield return new WaitForSeconds(2f);
        switch (player)
        {
            case 1:
                Player2HasWonText.SetActive(true);
                break;
            case 2:
                Player1HasWonText.SetActive(true);
                break;
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
