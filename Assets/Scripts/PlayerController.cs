using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour {

    [Header("Player Parameters")]
    [SerializeField] float moveSpeed = 1;
    bool canRespond = false;

    [Header("Fart Stuff")]
    [SerializeField] float requiredFartHoldStrength = 0.8f;
    [SerializeField] float fartRate = 10f;
    [SerializeField] float fartThreshold = 1f;
    [SerializeField] GameObject fartPrefab;
    [SerializeField] GameObject fartUIPivot;
    [SerializeField] GameObject leftTriggerUI;
    [SerializeField] GameObject rightTriggerUI;


    [Header("Dialogue Stuff")]
    [SerializeField] Canvas dialogueCanvas;

    bool level1Released;
    bool level2Released;
    bool level3Released;

    //[Header("Audio Files")]


    Player rewiredPlayer;
    Joystick playerJoystick;
    Rigidbody2D rb2d;

    float timeSinceLastFart = 0f;
    bool hasFart = false;
    bool holdingFart = false;
    float fartLevel = 0f;

	void Start ()
    {
        rewiredPlayer = ReInput.players.GetPlayer(0);
        playerJoystick = rewiredPlayer.controllers.Joysticks[0];
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
        dialogueCanvas.gameObject.SetActive(false);
        leftTriggerUI.SetActive(false);
        rightTriggerUI.SetActive(false);
    }
	
    void Update ()
    {
        PlayerActions();
        FartProducer();
        UpdateFart();
    }

	void FixedUpdate ()
    {
        PlayerMovement();
	}

    void PlayerMovement()
    {
        Vector2 moveVector = Vector2.zero;

        if (rewiredPlayer.GetAxis("Horizontal") < -0.1f)
        {
            moveVector += Vector2.left;

            if (this.transform.localScale.x < 0)
            {
                this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
                dialogueCanvas.transform.localScale = new Vector3(-dialogueCanvas.transform.localScale.x, dialogueCanvas.transform.localScale.y, dialogueCanvas.transform.localScale.z);
            }
        }
        else if (rewiredPlayer.GetAxis("Horizontal") > 0.1f)
        {
            moveVector += Vector2.right;

            if (this.transform.localScale.x > 0)
            {
                this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
                dialogueCanvas.transform.localScale = new Vector3(-dialogueCanvas.transform.localScale.x, dialogueCanvas.transform.localScale.y, dialogueCanvas.transform.localScale.z);
            }
         }

        if (rewiredPlayer.GetAxis("Vertical") > 0.1f)
        {
            moveVector += Vector2.up;
        }
        else if(rewiredPlayer.GetAxis("Vertical") < -0.1f)
        {
            moveVector += Vector2.down;
        }

        rb2d.MovePosition(rb2d.position + moveSpeed * moveVector * Time.deltaTime);
    }
       
    void PlayerActions ()
    {
        if (hasFart)
        {
            float leftTrigger = rewiredPlayer.GetAxis("LTrigger");
            float rightTrigger = rewiredPlayer.GetAxis("RTrigger");

            if (!holdingFart)
            {
                // Hold fart
                if (leftTrigger > requiredFartHoldStrength && rightTrigger > requiredFartHoldStrength)
                {
                    holdingFart = true;
                    ChangeFartLevel(fartLevel * 0.5f);
                    leftTriggerUI.SetActive(false);
                    rightTriggerUI.SetActive(false);

                    level1Released = false;
                    level2Released = false;
                    level3Released = false;
                }
            }
            else if(holdingFart && leftTrigger < requiredFartHoldStrength && rightTrigger < requiredFartHoldStrength)
            {
                // Measure speed of release of fart
                float level1Threshold = 0.7f;
                float level2Threshold = 0.4f;
                float level3Threshold = 0.2f;

                if (leftTrigger < level1Threshold && leftTrigger >level2Threshold && 
                    rightTrigger < level1Threshold && rightTrigger > level2Threshold && !level1Released) { level1Released = true; }

                if (leftTrigger < level2Threshold && leftTrigger > level3Threshold && 
                    rightTrigger < level2Threshold && rightTrigger > level3Threshold && !level2Released) { level2Released = true; }

                if (leftTrigger < level3Threshold && rightTrigger < level3Threshold && !level3Released) { level3Released = true; }

                if (leftTrigger == 0 && rightTrigger == 0)
                {
                    if (level1Released && level2Released && level3Released)
                    {
                        Debug.Log("quiet fart...");
                        StartCoroutine(Fart(0f));
                    }
                    else
                    {
                        Debug.Log("LOUD FART!");
                        StartCoroutine(Fart(fartLevel));
                    }
                }
            }
        }

        if(canRespond)
        {
            if(rewiredPlayer.GetButtonDown("XButton"))
            {
                Debug.Log("X Button");    
            }
            else if (rewiredPlayer.GetButtonDown("YButton"))
            {
                Debug.Log("Y Button");
            }
            else if (rewiredPlayer.GetButtonDown("AButton"))
            {
                Debug.Log("A Button");
            }
            else if (rewiredPlayer.GetButtonDown("BButton"))
            {
                Debug.Log("B Button");
            }
        }
    }
    
    void FartProducer ()
    {
        timeSinceLastFart += Time.deltaTime;

        if(timeSinceLastFart > fartRate && hasFart == false)
        {
            // Create fart
            hasFart = true;
            ChangeFartLevel(0.05f);
            fartUIPivot.transform.rotation = Quaternion.Euler(0, 0, 180);
            leftTriggerUI.SetActive(true);
            rightTriggerUI.SetActive(true);
            level1Released = false;
            level2Released = false;
            level3Released = false;
        }
        else if(hasFart == true)
        {
            timeSinceLastFart = 0;
        }
    }
    
    void UpdateFart ()
    {
        if (hasFart)
        {
            float newFartLevel = fartLevel;
            
            if (holdingFart)
            {
                newFartLevel += 0.001f;

                if (fartUIPivot.transform.eulerAngles.z > 0)
                {
                    fartUIPivot.transform.Rotate(Vector3.back);
                }
            }
            else
            {
                newFartLevel += 0.002f;

                if (fartUIPivot.transform.eulerAngles.z > 0)
                {
                    fartUIPivot.transform.Rotate(Vector3.back);
                }
            }        

            if(newFartLevel > 0.80f * fartThreshold)
            {
                dialogueCanvas.gameObject.SetActive(true);
                dialogueCanvas.GetComponentInChildren<Text>().text = "I can't hold it much longer!";
            }
            else if(newFartLevel > fartThreshold)
            {
                StartCoroutine(Fart(1.5f));
            }
            else
            {
                ChangeFartLevel(newFartLevel);
            }
        }
    }

    void ChangeFartLevel(float newFartLevel)
    {

        fartLevel = newFartLevel;
        playerJoystick.SetVibration(fartLevel, fartLevel);
    }

    IEnumerator Fart (float newFartLevel)
    {

        Instantiate(fartPrefab, this.transform.position, Quaternion.identity);

        // Create audio

        hasFart = false;
        holdingFart = false;

        ChangeFartLevel(newFartLevel);

        yield return new WaitForSeconds(1f);
                
        ChangeFartLevel(0);
        fartUIPivot.transform.rotation = Quaternion.Euler(0, 0, 180);
        leftTriggerUI.SetActive(false);
        rightTriggerUI.SetActive(false);
    }
}
