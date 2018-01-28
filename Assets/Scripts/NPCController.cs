using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class NPCController : MonoBehaviour {

    [Header("Navigation")]
    [SerializeField] float moveSpeed = 2;
    [SerializeField] float restTimeMin = 3;
    [SerializeField] float restTimeMax = 10;
    [SerializeField] float distanceRequired = 0.5f;
    [SerializeField] GameObject[] waypoints;
    int waypointIndex = 0;
    bool resting = false;

    [Header("Dialogue")]
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] GameObject NPCEmote;
    [SerializeField] string[] fartReactionArray;
    [SerializeField] string[] playerCalloutArray;

    Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {

        rb2d = this.transform.GetComponentInChildren<Rigidbody2D>();
        dialogueCanvas.gameObject.SetActive(false);

        if (waypoints.Length == 0) { resting = true; }
        else { this.transform.position = waypoints[waypointIndex].transform.position; }
	}
	
	// Update is called once per frame
	void Update () {

        if(!resting)
        {
            // Check how far the NPC is to their destination waypoint
            float distance = Vector3.Distance(this.transform.position, waypoints[waypointIndex].transform.position);
            Vector2 moveVector = waypoints[waypointIndex].transform.position - this.transform.position;

            // Flip sprite if necessary
            if((waypoints[waypointIndex].transform.position.x > this.transform.position.x && this.transform.localScale.x > 0) ||
                (waypoints[waypointIndex].transform.position.x < this.transform.position.x && this.transform.localScale.x < 0))
            {
                this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
                dialogueCanvas.GetComponentInChildren<Text>().transform.localScale = new Vector3(-dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.x, dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.y, dialogueCanvas.GetComponentInChildren<Text>().transform.localScale.z);
            }
        
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

    void GetAngry()
    {
        NPCEmote.GetComponent<Animator>().SetBool("IsAngry", true);
        StartCoroutine(CalmDown());
    }

    IEnumerator CalmDown()
    {
        yield return new WaitForSeconds(5);
        NPCEmote.GetComponent<Animator>().SetBool("IsAngry", false);
    }

    IEnumerator WaitAtWaypoint ()
    {
        resting = true;

        yield return new WaitForSeconds(Random.Range(restTimeMin, restTimeMax));

        resting = false;

        waypointIndex++;
        if (waypointIndex >= waypoints.Length) { waypointIndex = 0; }
    }

    public void SmellFart ()
    {
        GetAngry();
        dialogueCanvas.gameObject.SetActive(true);
        dialogueCanvas.GetComponentInChildren<Text>().text = fartReactionArray[Random.Range(0, fartReactionArray.Length)];
        StartCoroutine(HideDialogueBox());
    }

    public void SayHi()
    {
        dialogueCanvas.gameObject.SetActive(true);
        dialogueCanvas.GetComponentInChildren<Text>().text = playerCalloutArray[Random.Range(0, playerCalloutArray.Length)];
        StartCoroutine(HideDialogueBox());
    }

    IEnumerator HideDialogueBox ()
    {
        yield return new WaitForSeconds(3);
        dialogueCanvas.gameObject.SetActive(false);
    }
}
