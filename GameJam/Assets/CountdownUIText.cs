using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUIText : MonoBehaviour
{
    public Text textComp;
    public GameObject Panel;
    void Start()
    {
        StartCoroutine(Countdown());
        //textComp = gameObject.GetComponent<Text>();
    }

    IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {

            textComp.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        gameObject.GetComponentInParent<Transform>().gameObject.SetActive(false);
        Panel.SetActive(false);
}



}
