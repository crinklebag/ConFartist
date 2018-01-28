using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class NPCController : MonoBehaviour {

    [SerializeField] float moveSpeed = 2;
    [SerializeField] float restTimeMin = 3;
    [SerializeField] float restTimeMax = 10;

    [SerializeField] float distanceRequired = 0.5f;
    [SerializeField] GameObject[] waypoints;
    int waypointIndex = 0;
    bool resting = false;

    Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {

        rb2d = this.transform.GetComponentInChildren<Rigidbody2D>();
        this.transform.position = waypoints[waypointIndex].transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if(!resting)
        {
            // Check how far the NPC is to their destination waypoint
            float distance = Vector3.Distance(this.transform.position, waypoints[waypointIndex].transform.position);
            Vector2 moveVector = waypoints[waypointIndex].transform.position - this.transform.position;
        
            if(distance > distanceRequired)
            {
                rb2d.MovePosition(rb2d.position + moveSpeed * moveVector.normalized * Time.deltaTime);
            }

            else if(distance < distanceRequired)
            {
                StartCoroutine(WaitAtWaypoint());
            }
        }
	}

    IEnumerator WaitAtWaypoint ()
    {
        resting = true;

        yield return new WaitForSeconds(Random.Range(restTimeMin, restTimeMax));

        resting = false;

        waypointIndex++;
        if (waypointIndex >= waypoints.Length) { waypointIndex = 0; }
    }
}
