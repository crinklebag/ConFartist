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
    bool initialFartThought = false;
    bool fartWarningGiven = false;


    [Header("Dialogue Stuff")]
    [SerializeField] Canvas dialogueCanvas;

    bool level1Released;
    bool level2Released;
    
    Player rewiredPlayer;
    Joystick playerJoystick;
    Rigidbody2D rb2d;

    Animator[] playerAnimators;

    float timeSinceLastFart = 0f;
    bool hasFart = false;
    bool holdingFart = false;
    float fartLevel = 0f;

	void Start ()
    {
        rewiredPlayer = ReInput.players.GetPlayer(0);
        playerJoystick = rewiredPlayer.controllers.Joysticks[0];
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
        playerAnimators = this.GetComponentsInChildren<Animator>();
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
                dialogueCanvas.GetComponentInChildren<Text>().transform.localScale = new Vector3(-dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.x, dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.y, dialogueCanvas.transform.localScale.z);
            }
        }
        else if (rewiredPlayer.GetAxis("Horizontal") > 0.1f)
        {
            moveVector += Vector2.right;

            if (this.transform.localScale.x > 0)
            {
                this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
                dialogueCanvas.GetComponentInChildren<Text>().transform.localScale = new Vector3(-dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.x, dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.y, dialogueCanvas.transform.localScale.z);
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

        if (moveVector != Vector2.zero)
        {
            foreach (Animator animator in playerAnimators)
            {
                animator.SetBool("isWalking", true);
            }
            rb2d.MovePosition(rb2d.position + moveSpeed * moveVector * Time.deltaTime);
        }
        else
        {
            foreach (Animator animator in playerAnimators)
            {
                animator.SetBool("isWalking", false);
            }
        }
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
                }
            }
            else if(holdingFart && leftTrigger < requiredFartHoldStrength && rightTrigger < requiredFartHoldStrength)
            {
                // Measure speed of release of fart
                float level1Threshold = 0.7f;
                float level2Threshold = 0.3f;

                if (leftTrigger < level1Threshold && leftTrigger >level2Threshold && 
                    rightTrigger < level1Threshold && rightTrigger > level2Threshold && !level1Released)
                {
                    level1Released = true;
                    playerJoystick.SetVibration(0.5f * fartLevel, 0.5f * fartLevel);
                }

                if (leftTrigger < level2Threshold && rightTrigger < level2Threshold && !level2Released)
                {
                    level2Released = true;
                    playerJoystick.SetVibration(0.5f * fartLevel, 0.5f * fartLevel);
                }

                if (leftTrigger == 0 && rightTrigger == 0)
                {
                    if (level1Released && level2Released)
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
            
            // Update Fart UI
            if (holdingFart)
            {
                newFartLevel += 0.001f;
            }
            else
            {
                newFartLevel += 0.002f;
            }

            fartUIPivot.transform.eulerAngles = new Vector3(0, 0, 180 - ((newFartLevel / fartThreshold) * 180));

            // Fart Thoughts
            if(initialFartThought == false)
            {
                initialFartThought = true;
                dialogueCanvas.gameObject.SetActive(true);
                dialogueCanvas.GetComponentInChildren<Text>().text = "Uh-oh, I have to fart...";
                StartCoroutine(HideDialogue());
            }
            if (newFartLevel > 0.80f * fartThreshold && !fartWarningGiven)
            {
                fartWarningGiven = true;
                dialogueCanvas.gameObject.SetActive(true);
                dialogueCanvas.GetComponentInChildren<Text>().text = "I can't hold it much longer!";
                StartCoroutine(HideDialogue());
            }


            // Fart Results
            if(newFartLevel > fartThreshold)
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

        GameObject newFart = Instantiate(fartPrefab, this.transform.position + new Vector3(0, -1), Quaternion.identity) as GameObject;
        bool passedVolume = (newFartLevel >= fartThreshold) ? true : false;
        newFart.GetComponentInChildren<Fart>().SetVolume(true);

        hasFart = false;
        holdingFart = false;
        fartWarningGiven = false;

        ChangeFartLevel(newFartLevel);

        yield return new WaitForSeconds(1f);
                
        ChangeFartLevel(0);
        fartUIPivot.transform.rotation = Quaternion.Euler(0, 0, 180);
        leftTriggerUI.SetActive(false);
        rightTriggerUI.SetActive(false);
    }

    IEnumerator HideDialogue ()
    {
        yield return new WaitForSeconds(1);

        dialogueCanvas.gameObject.SetActive(false);
    }
}
