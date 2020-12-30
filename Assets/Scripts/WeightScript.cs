using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightScript : MonoBehaviour
{
    public int gravityScaleTime;
    private Rigidbody2D rigidBody;
    private LevelManager gameLevelManager;
    public Transform groundCheckPoint;
    public Transform groundCheckPoint1;
    public Transform groundCheckPoint2;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    private bool isTouchingGround;
    private bool isTouchingGround1;
    private bool isTouchingGround2;
    public int weightFadeTime;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        gameLevelManager = FindObjectOfType<LevelManager>();
        StartCoroutine("IncreaseGravity");
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position,groundCheckRadius,groundLayer);
        isTouchingGround1 = Physics2D.OverlapCircle(groundCheckPoint1.position,groundCheckRadius,groundLayer);
        isTouchingGround2 = Physics2D.OverlapCircle(groundCheckPoint2.position,groundCheckRadius,groundLayer);
        if(isTouchingGround || isTouchingGround1 || isTouchingGround2){
            Destroy(gameObject);
        }
    }

    private IEnumerator IncreaseGravity()
    {
        yield return new WaitForSeconds(gravityScaleTime);
        if(gameLevelManager.gravityScale<2){
            gameLevelManager.IncreaseWeightGravity(0.08f);
        }else if(gameLevelManager.gravityScale>=2 && gameLevelManager.gravityScale<5){
            gameLevelManager.IncreaseWeightGravity(0.05f);
        }else if(gameLevelManager.gravityScale>=5 && gameLevelManager.gravityScale<8){
            gameLevelManager.IncreaseWeightGravity(0.03f);
        }else if(gameLevelManager.gravityScale>=8 && gameLevelManager.gravityScale<10){
            gameLevelManager.IncreaseWeightGravity(0.01f);
        }
        rigidBody.gravityScale = gameLevelManager.gravityScale;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "FallDetector"){
            Destroy(gameObject);
        }
    }

    
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Player"){
            StartCoroutine("DestroyWeight");
        }
    }

    public IEnumerator DestroyWeight(){
        yield return new WaitForSeconds(weightFadeTime);
        Destroy(gameObject);
    }
}
