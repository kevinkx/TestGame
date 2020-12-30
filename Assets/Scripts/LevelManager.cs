using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float respawnDelay;
    public float maxCoinDelay;
    public float weightDelay;
    public float gravityScale;
    public int restartTimer;
    public PlayerController gamePlayer;
    public int coins;
    public GameObject coinPrefabs;
    public GameObject weightPrefabs;
    public Transform leftDetector;
    public Transform rightDetector;
    public Transform skyDetector;
    public Text coinText;
    public Text highScore;
    public Text lastScore;
    public Text worldRecord;
    public Text timerDashCooldownText;
    private int timerDashCooldown = 0;
    private int timerDashDuration = 0;
    private int tempTimerDashCooldown;
    public Text timerBunshinCooldownText;
    private int timerBunshinCooldown = 0;
    private int timerBunshinDuration = 0;
    private int tempTimerBunshinCooldown;
    public Sprite greyDash, Dash;
    public Sprite greyBunshin, Bunshin;
    public Sprite greyJump, Jump;
    public RectTransform rectTransformDashSprite;
    public RectTransform rectTransformBunshinSprite;
    public RectTransform rectTransformJumpSprite;
    private Image imageDashSprite;
    private Image imageBunshinSprite;
    private Image imageJumpSprite;
    private bool isJumpAvailable;
    private bool restartCalled = false;

    // Start is called before the first frame update
    void Start()
    {
        Uniredis.Get(this, "WorldRecord", (error, result) =>
        {
            if (error == null)
            {
                worldRecord.text = "World Record: " + result;
            }
        });

        Uniredis.Get(this, SystemInfo.deviceUniqueIdentifier+"HighScore", (error, result) =>
        {
            if (error == null)
            {
                highScore.text = "Your HighScore: " + result;
            }
        });

        Uniredis.Get(this, SystemInfo.deviceUniqueIdentifier+"LastScore", (error, result) =>
        {
            if (error == null)
            {
                lastScore.text = "Your LastScore: " + result;
            }
        });

        gamePlayer = FindObjectOfType<PlayerController>();
        coinText.text = "Coins: " + coins;
        StartCoroutine("CoinWave");
        StartCoroutine("WeightWave");
        timerDashCooldown = gamePlayer.cooldownDash;
        timerDashDuration = gamePlayer.durationDash;
        timerBunshinCooldown = gamePlayer.cooldownBunshin;
        timerBunshinDuration = gamePlayer.durationBunshin;
        timerDashCooldownText.text = ""+ timerDashCooldown;
        timerDashCooldownText.enabled = false;
        timerBunshinCooldownText.text = ""+ timerBunshinCooldown;
        timerBunshinCooldownText.enabled = false;
        imageDashSprite = rectTransformDashSprite.GetComponent<Image>();
        imageBunshinSprite = rectTransformBunshinSprite.GetComponent<Image>();
        imageJumpSprite = rectTransformJumpSprite.GetComponent<Image>();
        isJumpAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gamePlayer==null && restartCalled == false){
            StartCoroutine("LoadRestartGame");
            restartCalled = true;
        }else{
            // for logic jump button active to de-active, vice versa.
            if(gamePlayer.isTouchingGround == true && isJumpAvailable == false){
                imageJumpSprite.sprite = Jump;
                isJumpAvailable = true;
            }
            else if(gamePlayer.isTouchingGround == false && isJumpAvailable){
                imageJumpSprite.sprite = greyJump;
                isJumpAvailable = false;
            }
        } 
    }
    
    public void Respawn(){
        StartCoroutine("RespawnCoRoutine");
    }

    public IEnumerator RespawnCoRoutine()
    {
        gamePlayer.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        gamePlayer.transform.position = gamePlayer.respawnPoint;
        gamePlayer.gameObject.SetActive(true);
    }

    
    public void IncreaseWeightGravity(float addGravity){
        gravityScale += addGravity;
    }

    public void AddCoins(int numberOfCoins){
        coins += numberOfCoins;
        coinText.text = "Coins: " + coins;
    }

    public void SpawnCoin(){
        GameObject c = Instantiate(coinPrefabs) as GameObject;
        c.transform.position = new Vector2(Random.Range(leftDetector.position.x, rightDetector.position.x), skyDetector.position.y);
    }

    public IEnumerator CoinWave(){
        while(true){
            yield return new WaitForSeconds(Random.Range(1, maxCoinDelay));
            SpawnCoin();
        }
    }
    
    public void SpawnWeight(){
        GameObject w = Instantiate(weightPrefabs) as GameObject;
        w.transform.position = new Vector2(Random.Range(leftDetector.position.x, rightDetector.position.x), skyDetector.position.y);
    }

    public IEnumerator WeightWave(){
        while(true){
            yield return new WaitForSeconds(weightDelay);
            SpawnWeight();
            if(weightDelay>1f){
                weightDelay = weightDelay - 0.1f;
            }
            else if(weightDelay<=1f && weightDelay>0.5f){
                weightDelay = weightDelay - 0.001f;
            }
        }
    }

    public void StartTimerDashCooldown(){
        if(tempTimerDashCooldown == 0){
            tempTimerDashCooldown = timerDashCooldown;
            timerDashCooldownText.enabled = true;
            imageDashSprite.sprite = greyDash;
        }
        timerDashCooldownText.text = ""+ timerDashCooldown;
        if(timerDashCooldown>0){
            StartCoroutine("TimerDashCountdown");
        }else{
            timerDashCooldown = tempTimerDashCooldown;
            tempTimerDashCooldown = 0;
            timerDashCooldownText.enabled = false;
            imageDashSprite.sprite = Dash;
        }
    }

    public IEnumerator TimerDashCountdown()
    {
        yield return new WaitForSeconds(1);
        timerDashCooldown -= 1;
        StartTimerDashCooldown();
    }


    public void StartTimerBunshinCooldown(){
        if(tempTimerBunshinCooldown == 0){
            tempTimerBunshinCooldown = timerBunshinCooldown;
            timerBunshinCooldownText.enabled = true;
            imageBunshinSprite.sprite = greyBunshin;
        }
        timerBunshinCooldownText.text = ""+ timerBunshinCooldown;
        if(timerBunshinCooldown>0){
            StartCoroutine("TimerBunshinCountdown");
        }else{
            timerBunshinCooldown = tempTimerBunshinCooldown;
            tempTimerBunshinCooldown = 0;
            timerBunshinCooldownText.enabled = false;
            imageBunshinSprite.sprite = Bunshin;
        }
    }
    
    public IEnumerator TimerBunshinCountdown()
    {
        yield return new WaitForSeconds(1);
        timerBunshinCooldown -= 1;
        StartTimerBunshinCooldown();
    }

    public IEnumerator LoadRestartGame()
    {
        Uniredis.Set(this, SystemInfo.deviceUniqueIdentifier+"LastScore", coins, (error, result) =>
        {
            if (error == null)
            {
                Debug.Log("SET LASTSCORE SUCCESS");
            }
        });
        Uniredis.Get(this, SystemInfo.deviceUniqueIdentifier+"HighScore", (error, result) =>
        {
            if (error == null)
            {
                if(int.Parse(result)<coins){
                    Uniredis.Set(this, SystemInfo.deviceUniqueIdentifier+"HighScore", coins, (error1, result1) =>
                    {
                        if (error == null)
                        {
                            Debug.Log("SET HIGHSCORE SUCCESS");
                        }
                    });
                }
            }
        });
        Uniredis.Get(this, "WorldRecord", (error, result) =>
        {
            if (error == null)
            {
                if(int.Parse(result)<coins){
                    Uniredis.Set(this, "WorldRecord", coins, (error1, result1) =>
                    {
                        if (error == null)
                        {
                            Debug.Log("SET WORLDRECORD SUCCESS");
                        }
                    });
                }
            }
        });
        yield return new WaitForSeconds(restartTimer);
        RestartGame();
    }

    public void RestartGame(){
        SceneManager.LoadScene("Level01");
    }

}
