using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskAnimator : MonoBehaviour
{
   
    public Sprite[] frames;
    public float fps;
    public float hold1;
    public float hold2;
    public bool randomize;
    public int sync;
    public GameObject syncTarget;
    public bool thereIsASync;

    void Start()
    {
        if (syncTarget != null)
            thereIsASync = true;
        else
            thereIsASync = false;
        print(thereIsASync);
        if (randomize)
            StartCoroutine(Randomizer());
        else
            StartCoroutine(Animate());
    }

    private void OnEnable()
    {
        print("object is enabled");
        if (randomize)
            StartCoroutine(Randomizer());
        else
            StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (gameObject.activeSelf)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                gameObject.GetComponent<SpriteMask>().sprite = frames[i];
                sync = i;
                yield return new WaitForSeconds(fps);
            }
            yield return new WaitForSeconds(hold1);
            for (int i = frames.Length - 1; i > -1; i--)
            {
                gameObject.GetComponent<SpriteMask>().sprite = frames[i];
                sync = i;
                yield return new WaitForSeconds(fps);
            }
            yield return new WaitForSeconds(hold2);
        }
    }

    IEnumerator Randomizer()
    {
        while (gameObject.activeSelf)
        {
            int i = Random.Range(0, frames.Length - 1);
            gameObject.GetComponent<SpriteMask>().sprite = frames[i];
            sync = i;
            yield return new WaitForSeconds(fps);
        }
    }
}
