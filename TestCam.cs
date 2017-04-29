using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCam : MonoBehaviour {

	public float moveSpeed=5;
	public float rotationSpeed=5;

	void Awake () 
	{
		GameMaster.cam = GetComponent<Camera>();
	}
	// Update is called once per frame
	void Update () {
		float angle = Input.GetAxis("Mouse X");
		if (angle != 0) 
		{
			transform.Rotate(Vector3.up*angle*rotationSpeed*Time.deltaTime,Space.World);
		}
		angle = Input.GetAxis("Mouse Y");
		if (angle != 0) 
		{
			transform.Rotate(Vector3.left*angle*rotationSpeed*Time.deltaTime,Space.Self);
		}

		Vector3 movement = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
		if (movement != Vector3.zero)
		{
			transform.Translate(movement*moveSpeed*Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.Space))
		{
			transform.Translate(Vector3.up*moveSpeed*Time.deltaTime,Space.World);
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			transform.Translate(Vector3.down*moveSpeed*Time.deltaTime,Space.World);
		}

		if (Input.GetMouseButtonDown(0))
		{
			Ray r=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rh;
			if (Physics.Raycast(r,out rh))
			{					
				
			}
		}
	}
}
