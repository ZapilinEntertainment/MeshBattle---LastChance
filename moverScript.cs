using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverScript : MonoBehaviour {
	public float speed = 2;
	public bool move = true;
	public Vector3 rotationVector;

	
	// Update is called once per frame
	void Update () {
		if (!move || GameMaster.IsPaused()) return;
		transform.Translate(Vector3.forward * speed *Time.deltaTime);
		transform.Rotate(rotationVector *Time.deltaTime, Space.Self);
	}
}
