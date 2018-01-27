using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour {

    [SerializeField] float moveSpeed = 1;
    [SerializeField] float requiredFartHoldStrength = 0.8f;

    Player rewiredPlayer;
    Rigidbody2D rb2d;

    float timeSinceLastFart = 0f;
    float currentTime = 0f;
    float fartRate = 10f;
    bool hasFart = false;

	void Start ()
    {
        rewiredPlayer = ReInput.players.GetPlayer(0);
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
    }
	
    void Update ()
    {
        PlayerActions();
        FartProducer();
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
        }
        if (rewiredPlayer.GetAxis("Horizontal") > 0.1f)
        {
            moveVector += Vector2.right;
        }

        if (rewiredPlayer.GetAxis("Vertical") > 0.1f)
        {
            moveVector += Vector2.up;
        }
        if(rewiredPlayer.GetAxis("Vertical") < -0.1f)
        {
            moveVector += Vector2.down;
        }

        rb2d.MovePosition(rb2d.position + moveSpeed * moveVector * Time.deltaTime);
    }
       
    void PlayerActions ()
    {
        if(rewiredPlayer.GetAxis("LTrigger") > requiredFartHoldStrength && rewiredPlayer.GetAxis("RTrigger") > requiredFartHoldStrength)
        {

            Debug.Log("HOLDING IN THAT FART");
        }
    }
    
    void FartProducer ()
    {
        timeSinceLastFart += Time.deltaTime;

        if(timeSinceLastFart > fartRate)
        {
            // Create fart
            hasFart = true;
            Debug.Log("FART");
        }
    }

    public IEnumerator Rumble(float duration)
    {
        Joystick joystick = rewiredPlayer.controllers.Joysticks[0];
        float rumbleIntensity = 1.0f;
        joystick.SetVibration(rumbleIntensity, rumbleIntensity);
        
        yield return new WaitForSeconds(duration);

        joystick.StopVibration();
    }
}
