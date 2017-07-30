using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Destructible {

	public float MOVEMENT_ANGLE_LIMIT;
	public float ANGLE_THRESHOLD;
	public float START_SMOOTH_COEFFICIENT = 0.1f;
	public float ROTATION_SMOOTH_ACCELERATION = 0.1f;

	protected float speed = 0;
	public float acceleration = 0.5f;
	protected float speedGoal = 0;
	public float maxSpeed = 10;
	public float rotationSpeed = 25;
	float rotationSmoothCoefficient;
	public bool usePooling = true;

	public bool rotating = false;
	Rigidbody rbody;
	Quaternion rotateTo;
	Vector3 rotatingVector = Vector3.zero;


	void Awake () 
	{
		colliders = gameObject.GetComponents<BoxCollider>();
		rbody = GetComponent<Rigidbody>();
		hp = maxHp;
		armor = maxArmor;
		pooling = usePooling;
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}

	void Update ()
	{
		if (GameMaster.pause) return;

		float t = Time.deltaTime;
		float rspeed = rotationSpeed * rotationSmoothCoefficient * t;

		if (rotating) {
		if (rotateTo != transform.rotation) transform.rotation = Quaternion.RotateTowards (transform.rotation, rotateTo, rspeed);
			else rotating = false;
			rotationSmoothCoefficient += ROTATION_SMOOTH_ACCELERATION * t;
			if (rotationSmoothCoefficient > 1) rotationSmoothCoefficient = 1;
		}
		else {
			if (rotatingVector!= Vector3.zero) {
				transform.Rotate(rotatingVector * rspeed, Space.Self);
				rotationSmoothCoefficient += ROTATION_SMOOTH_ACCELERATION * t;
				if (rotationSmoothCoefficient > 1) rotationSmoothCoefficient = 1;
			}
		}

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
		if (speed > 0) 	transform.Translate(transform.forward * speed * t, Space.World);


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
		
	public override void Destruction () 
	{
		if (destroyed) return; else destroyed=true;
		if (myRenderers) myRenderers.SetActive(false);


		foreach (BoxCollider c in colliders) {
			if (c.size.magnitude > 10) GameMaster.pool.DestructionAt (c, Vector3.zero);
			else GameMaster.pool.PiecesAt(transform.position, (int)c.size.magnitude);
			c.enabled = false;
		}

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
			if (angle > ANGLE_THRESHOLD) 	{
				rotateTo = Quaternion.LookRotation(goalDirection,transform.TransformDirection(Vector3.up)); 
				rotating = true;
			}
	    }
		else speedGoal = 0;
	}
	public void SetSpeed(float t) {if (t < 0) t = 0; if (t > 1) t = 1; speedGoal = t * maxSpeed;}

	public void RotateTo (Quaternion q) 
	{
		rotatingVector = Vector3.zero;
		rotateTo = q;
		rotating = true;
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}
	public void SetRotateVector(Vector3 t) 
	{
		rotatingVector = t;
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}
	public Vector3 GetRotateVector() {return rotatingVector;}
	public void SetYRotation (float t) 
	{
		rotating = false;
		rotatingVector.y = System.Math.Sign(t); 
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}
	public void SetXRotation( float t) {
		rotating = false;
		rotatingVector.x = System.Math.Sign(t);
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}
	public void SetZRotation( float t) {
		rotating = false;
		rotatingVector.z = System.Math.Sign(t);
		rotationSmoothCoefficient = START_SMOOTH_COEFFICIENT;
	}


}
