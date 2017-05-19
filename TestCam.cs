using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCam : MonoBehaviour {

	public float moveSpeed=5;
	public float rotationSpeed=5;
	public Transform bindObject;
	Vector3 deltaPos;

	void Awake () 
	{
		GameMaster.cam = GetComponent<Camera>();
		if (bindObject) deltaPos = transform.position - bindObject.position;
	}
	// Update is called once per frame
	void LateUpdate () {

		if (bindObject) transform.position = bindObject.position + deltaPos;

		if (Input.GetMouseButton(1)) {
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
		}
		else {
			if (Input.GetMouseButton(2)) {
				Vector3 movement = new Vector3( -Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
				transform.Translate(movement*moveSpeed*Time.deltaTime, Space.Self);
			}
		}

		float msw = Input.GetAxis("Mouse ScrollWheel") * 30;
		if (msw != 0) transform.Translate(Vector3.forward*msw*moveSpeed*Time.deltaTime, Space.Self);

		if (bindObject) deltaPos = transform.position - bindObject.position;

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
