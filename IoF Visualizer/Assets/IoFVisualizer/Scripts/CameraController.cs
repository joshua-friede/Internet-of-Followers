using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    private Vector3 rot = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float strafeHorizontal = Input.GetAxis("Strafe Horizontal");
        float moveFwd = Input.GetAxis("Fwd/Back");
        float moveVertical = Input.GetAxis("Move Vertical");
        float turnHorizontal = Input.GetAxis("Turn Horizontal");
        float turnVertical = Input.GetAxis("Turn Vertical");

        this.transform.position += this.moveSpeed * moveFwd * (this.transform.rotation * Vector3.forward) * Time.deltaTime;
        this.transform.position += this.moveSpeed * strafeHorizontal * (this.transform.rotation * Vector3.right) * Time.deltaTime;
        this.transform.position += this.moveSpeed * moveVertical * (this.transform.rotation * Vector3.up) * Time.deltaTime;

        rot.y += this.turnSpeed * turnHorizontal * Time.deltaTime;
        rot.x -= this.turnSpeed * turnVertical * Time.deltaTime;

        this.transform.rotation = Quaternion.Euler(rot);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
            Debug.Log("Changed VRSettings.enabled to: " + UnityEngine.VR.VRSettings.enabled);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
