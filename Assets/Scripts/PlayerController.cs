using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    public float speed = 7f;
    private float tempSpeed;
    private float tempJumpSpeed;
    public float jumpSpeed = 10f;
    public float dashSpeed;
    public float movementX = 0f;
    private Rigidbody2D rigidBody;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isTouchingGround;
    private Animator playerAnimator;
    public Vector3 respawnPoint;
    public LevelManager gameLevelManager;
    public SoundManager gameSoundManager;
    public int cooldownDash;
    public int durationDash;
    private bool dashAvailable = true;
    public int cooldownBunshin;
    public int durationBunshin;
    private bool bunshinAvailable = true;
    private bool inBunshinMode = false;
    public ParticleSystem dashEffect;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        respawnPoint = transform.position;
        gameLevelManager = FindObjectOfType<LevelManager>();
        gameSoundManager = FindObjectOfType<SoundManager>();
        dashEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position,groundCheckRadius,groundLayer);
        movementX = CrossPlatformInputManager.GetAxis("Horizontal");
        if(inBunshinMode){
            movementX = 0;
        }
        if (movementX > 0f) { //arrow right or D pressed (positif velocity)
            rigidBody.velocity = new Vector2(movementX*speed, rigidBody.velocity.y);
            transform.localScale = new Vector2(Math.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (movementX < 0f) { //arrow left or A pressed (negative velocity)
            rigidBody.velocity = new Vector2(movementX*speed, rigidBody.velocity.y);
            transform.localScale = new Vector2(-Math.Abs(transform.localScale.x), transform.localScale.y);
        } 
        if (movementX == 0f) { //character stopped
            rigidBody.velocity = new Vector2(movementX*speed, rigidBody.velocity.y);
        }
        if(CrossPlatformInputManager.GetButtonDown("Jump") && isTouchingGround && inBunshinMode == false){
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
            gameSoundManager.PlaySound("jump");
        }
        if(CrossPlatformInputManager.GetButtonDown("Fire1") && dashAvailable && inBunshinMode == false){
            tempSpeed = speed;
            tempJumpSpeed = jumpSpeed;
            speed = dashSpeed;
            jumpSpeed = dashSpeed;
            dashAvailable = false;
            dashEffect.Play();
            StartCoroutine("DashDuration");
            StartCoroutine("DashCooldown");
            gameSoundManager.PlaySound("dash");
            gameLevelManager.StartTimerDashCooldown();
        }
        if(CrossPlatformInputManager.GetButtonDown("Fire2") && bunshinAvailable){
            inBunshinMode = true;
            bunshinAvailable = false;
            StartCoroutine("BunshinDuration");
            StartCoroutine("BunshinCooldown");
            gameSoundManager.PlaySound("bunshin");
            gameLevelManager.StartTimerBunshinCooldown();
        }
        playerAnimator.SetFloat("Speed", Math.Abs(rigidBody.velocity.x));
        playerAnimator.SetBool("OnGround", isTouchingGround);
        playerAnimator.SetBool("InBunshin", inBunshinMode);
    }

    public IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(durationDash);
        dashEffect.Stop();
        speed = tempSpeed;
        jumpSpeed = tempJumpSpeed;
    }

    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(cooldownDash);
        dashAvailable = true;
    }

    public IEnumerator BunshinDuration()
    {
        yield return new WaitForSeconds(durationBunshin);
        inBunshinMode = false;
    }

    public IEnumerator BunshinCooldown()
    {
        yield return new WaitForSeconds(cooldownBunshin);
        bunshinAvailable = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "FallDetector"){
            //gameLevelManager.Respawn();
            gameSoundManager.PlaySound("fallToLava");
            Destroy(gameObject);

        }
        if (other.tag == "CheckPoint"){
            respawnPoint = other.transform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.tag == "Obstacle" && inBunshinMode == false){
            gameSoundManager.PlaySound("die");
            Destroy(gameObject);
        }
    }

}
