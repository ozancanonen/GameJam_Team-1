using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private float currentSceneNumber;
    public float menuSceneNumber;
    public Slider loadingSlider;

    private void Start()
    {
        currentSceneNumber = SceneManager.GetActiveScene().buildIndex;   
    }

    private void Update()
    {
        if (currentSceneNumber == menuSceneNumber) // In the future, we need to make more menuSceneNumbers to make it work.
        {   
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }

    public void LoadLevel(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void LoadLevelAsync(string SceneName)
    {
        StartCoroutine(LoadAsync(SceneName));
    }

    public IEnumerator LoadAsync(string SceneName)
    {
        AsyncOperation operation =SceneManager.LoadSceneAsync(SceneName);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }

    }
    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit the game");
        Application.Quit();
    }

}
