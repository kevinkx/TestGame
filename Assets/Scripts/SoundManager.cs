using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip jumpSound, dashSound, collectCoinSound, bunshinSound, dieSound, fallToLavaSound;
    private AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void PlaySound(string clip){
        switch(clip){
            case "jump":
                audioSrc.PlayOneShot(jumpSound);
                break;
            case "dash":
                audioSrc.PlayOneShot(dashSound);
                break;
            case "collectCoin":
                audioSrc.PlayOneShot(collectCoinSound);
                break;
            case "bunshin":
                audioSrc.PlayOneShot(bunshinSound);
                break;
            case "die":
                audioSrc.PlayOneShot(dieSound);
                break;
            case "fallToLava":
                audioSrc.PlayOneShot(fallToLavaSound);
                break;
        }
    }
}
