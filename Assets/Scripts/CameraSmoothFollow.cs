using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraSmoothFollow : MonoBehaviour {

    [SerializeField] GameObject cameraTarget;
    [SerializeField] float offsetDistance = 10;
    [SerializeField] float offsetHeight = 0;
    [SerializeField] float smoothing = 2;

    Vector3 offset;
    Vector3 lastPosition;

    void Start()
    {
        if(!cameraTarget) { cameraTarget = GameObject.Find("Player"); }
        lastPosition = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, -offsetDistance);
        offset = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y + offsetHeight, -offsetDistance);
    }

    void Update()
    {

        //this.transform.position = offset;
        this.transform.position = new Vector3(
            Mathf.Lerp(lastPosition.x, cameraTarget.transform.position.x + offset.x, smoothing * Time.deltaTime),
            Mathf.Lerp(lastPosition.y, cameraTarget.transform.position.y + offset.y, smoothing * Time.deltaTime),
            -offsetDistance);
    }

    void LateUpdate()
    {
        lastPosition = this.transform.position;
    }
}
