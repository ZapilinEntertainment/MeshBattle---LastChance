﻿using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {

	Vector3 origin = Vector3.one;
	public float speed=0.0f;
	public Transform cam;
	public bool useGlobal = true;
	public float timer=0;
	public float time_left;
	bool use_timer=false;
	Vector3 start_scale;

	void Start () {
		if (useGlobal) cam=GameMaster.cam.transform;
		if (timer>0) {use_timer=true;time_left=timer;start_scale=transform.localScale;}
	}

	void Update () {
		if (!cam) return;
		transform.LookAt(cam.position);
		if (speed!=0) transform.localScale=origin*(1+Mathf.PingPong(Time.time*speed,0.3f));
		if (use_timer) {
			time_left-=Time.deltaTime;
			if (time_left>0) {transform.localScale=start_scale*time_left/timer;}
			else Destroy(transform.root.gameObject);
		}
	}    

	public void SetData (Vector3 newOrigin, float newSpeed, float newTime) 
	{
		if (newOrigin != null) origin = newOrigin;
		if (newSpeed > 0) speed = newSpeed;
		if (newTime > 0) {timer = newTime; use_timer = true;}
		else use_timer = false;
	}
		
}