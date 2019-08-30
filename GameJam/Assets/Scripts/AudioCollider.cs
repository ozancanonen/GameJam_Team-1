using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCollider : MonoBehaviour
{
    public string soundName;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponentInParent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag=="Player1"|| col.tag == "Player2" || col.tag == "Construct")
        audioManager.Play(soundName);
    }
}
