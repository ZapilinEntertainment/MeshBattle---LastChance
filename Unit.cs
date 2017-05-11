using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Destructible {

	public float MOVEMENT_ANGLE_LIMIT;
	public float ANGLE_THRESHOLD;

	public float speed = 0;
	public float acceleration = 0.5f;
	public float speedGoal = 0;
	public float maxSpeed = 10;
	public float rotationSpeed = 25;
	public bool usePooling = true;
	Rigidbody rbody;

	Quaternion rotateTo;

	void Awake () 
	{
		if (mainCollider == null) mainCollider = GetComponent<BoxCollider>();
		rbody = GetComponent<Rigidbody>();
		hp = maxHp;
		armor = maxArmor;
		pooling = usePooling;
	}

	void Update ()
	{
		if (GameMaster.pause) return;

		float t = Time.deltaTime;
			
		if (rotateTo != transform.rotation) transform.rotation = Quaternion.RotateTowards (transform.rotation, rotateTo, rotationSpeed*t);
		if (speedGoal > maxSpeed) speedGoal = maxSpeed; if (speedGoal < 0) speedGoal = 0;
		if (speedGoal > speed) {
			speed += acceleration * t;
			if (speed > speedGoal) speed = speedGoal;
		}
		else	{
			if (speedGoal < speed) {
				speed -= acceleration * t;
				if (speed < speedGoal) speed = speedGoal;
			}
		}
			
		if (speed>0) 	transform.Translate(Vector3.forward*speed*t);
		if (Vector3.Distance (transform.position, Vector3.zero) > GameMaster.mapRadius) Destruction();
		else {
			if (useTimer) 
			{
				timer -= Time.deltaTime;
				if (timer<=0) {
					timer = 0;
					useTimer = false;
					Destruction();
				}
			}
		}
	}
		
	public void Destruction () 
	{
		if (destroyed) return; else destroyed=true;
		if (myRenderers) myRenderers.SetActive(false);


		if (mainCollider.size.magnitude > 10) GameMaster.pool.DestructionAt (mainCollider, transform.TransformDirection(Vector3.forward* speed));
		else GameMaster.pool.PiecesAt(transform.position, (int)mainCollider.size.magnitude);

		transform.position = Vector3.zero;
		speed = 0;
		speedGoal = 0;
		rotateTo = transform.rotation;

		if (!pooling) Destroy(gameObject); else {transform.position = Vector3.zero;gameObject.SetActive(false);}
	}
		

	public void MoveTo(Vector3 point) 
	{
		Vector3 goalDirection = point - transform.position;
		float distance = goalDirection.magnitude;
		float angle = Vector3.Angle(transform.forward,goalDirection);
		if (distance > acceleration)
		{
			if (angle < MOVEMENT_ANGLE_LIMIT && speed*speed/2/acceleration<distance)  speedGoal = maxSpeed;  else speedGoal = 0;
		if (angle > ANGLE_THRESHOLD) 	rotateTo = Quaternion.LookRotation(goalDirection,transform.TransformDirection(Vector3.up));
	    }
		else speedGoal = 0;
	}

	public void RotateTo (Quaternion q) {rotateTo = q;}
		
}
