using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private LevelManager gameLevelManager;
    private SoundManager gameSoundManager;
    private Rigidbody2D rigidBody;
    public int coinValue;
    public int coinFadeTime;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        gameLevelManager = FindObjectOfType<LevelManager>();
        gameSoundManager = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player"){
            gameLevelManager.AddCoins(coinValue);
            gameSoundManager.PlaySound("collectCoin");
            Destroy(gameObject);
        }else if(other.tag != "Obstacle"){
            rigidBody.velocity = new Vector2(0, 0);
            rigidBody.gravityScale = 0f;
            StartCoroutine("DestroyCoin");
        }
        if (other.tag == "FallDetector"){
            Destroy(gameObject);
        }
    }

    public IEnumerator DestroyCoin(){
        yield return new WaitForSeconds(coinFadeTime);
        Destroy(gameObject);
    }
}
