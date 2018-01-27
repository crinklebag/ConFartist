using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour {

    [SerializeField] float moveSpeed = 1;

    Player rewiredPlayer;
    Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {

        rewiredPlayer = ReInput.players.GetPlayer(0);
        rb2d = this.GetComponentInChildren<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        PlayerInput();
	}

    void PlayerInput() {

        Vector2 moveVector = Vector2.zero;

        if (rewiredPlayer.GetAxis("Horizontal") < -0.1)
        {
            moveVector += Vector2.left;
        }
        if (rewiredPlayer.GetAxis("Horizontal") > 0.1)
        {
            moveVector += Vector2.right;
        }

        if (rewiredPlayer.GetAxis("Vertical") > 0.1)
        {
            moveVector += Vector2.up;
        }
        if(rewiredPlayer.GetAxis("Vertical") < -0.1)
        {
            moveVector += Vector2.down;
        }

        rb2d.MovePosition(rb2d.position + moveSpeed * moveVector * Time.deltaTime);
    }
}
