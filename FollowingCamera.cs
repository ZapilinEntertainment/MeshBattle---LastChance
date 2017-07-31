﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour {
	const float START_RT_SMOOTH_COEFFICIENT = 0.1f;
	const float START_ZM_SMOOTH_COEFFICIENT = 0.1f;

	public Transform obj;
	public Camera cam;
	public float rotationSpeed = 65;
	public float zoomSpeed = 50;
	float rotationSmoothCoefficient = START_RT_SMOOTH_COEFFICIENT;
	float rotationSmoothAcceleration = START_ZM_SMOOTH_COEFFICIENT;
	float zoomSmoothCoefficient = 0.1f;
	float zoomSmoothAcceleration = 0.1f;
	Vector3 deltaPos;

	void Start() {
		deltaPos = obj.position - transform.position;
		GameMaster.cam = cam;
	}
	// Update is called once per frame
	void LateUpdate () {
		if (obj == null || GameMaster.IsPaused()) return;
		//transform.position = obj.position + deltaPos;
		float delta = 0;
		if (Input.GetMouseButton(2)) {
			bool a = false , b = false; //rotation detectors
			float rspeed = rotationSpeed * Time.deltaTime * rotationSmoothCoefficient;
			delta = Input.GetAxis("Mouse X");
			if (delta != 0) {
				transform.RotateAround(obj.position, Vector3.up, rspeed * delta);
				rotationSmoothCoefficient += rotationSmoothAcceleration;
				a = true;
			}

			delta = Input.GetAxis("Mouse Y") ;
			if (delta != 0) {
				transform.RotateAround(obj.position, transform.TransformDirection(Vector3.left), rspeed * delta);
				rotationSmoothCoefficient += rotationSmoothAcceleration;
				b = true;
			}

			if (a==false && b== false) rotationSmoothCoefficient = START_RT_SMOOTH_COEFFICIENT;
		}

		delta = Input.GetAxis("Mouse ScrollWheel");
		if (delta != 0) {
			float zspeed = zoomSpeed * Time.deltaTime * zoomSmoothCoefficient * delta;
			transform.Translate((obj.position - transform.position) * zspeed, Space.World );
			zoomSmoothCoefficient += zoomSmoothAcceleration;
		}
		else zoomSmoothCoefficient = START_ZM_SMOOTH_COEFFICIENT;

		if (Input.GetMouseButtonUp(2)) {rotationSmoothCoefficient = START_RT_SMOOTH_COEFFICIENT;}

		deltaPos = obj.position - transform.position;
	}
}
