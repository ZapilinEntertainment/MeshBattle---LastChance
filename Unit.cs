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
	protected float contactRadius;
	Vector3 raycastPoint;
	Rigidbody rbody;

	Quaternion rotateTo;
	Order myOrder;

	void Awake () 
	{
		if (mainCollider == null) mainCollider = GetComponent<BoxCollider>();
		rbody = GetComponent<Rigidbody>();
		hp = maxHp;
		armor = maxArmor;
		pooling = usePooling;

		raycastPoint = new Vector3(0, mainCollider.center.y, mainCollider.center.z + mainCollider.size.z/2);
		contactRadius = mainCollider.size.magnitude * 2;
		if (myOrder == null) myOrder = new Order(OrderType.Stand);
	}

	void Update ()
	{
		if (GameMaster.pause) return;

		float t = Time.deltaTime;

		switch (myOrder.type)
		{
		case OrderType.Stand:
			if (speedGoal != 0) speedGoal = 0;
			break;
		}
			
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

	}
		
	public void Destruction () 
	{
		if (destroyed) return; else destroyed=true;
		if (myRenderers) myRenderers.SetActive(false);
		transform.position = Vector3.zero;
		speed = 0;
		speedGoal = 0;
		rotateTo = transform.rotation;
		myOrder = new Order(OrderType.Stand);

		if (mainCollider != null) GameMaster.pool.DestructionAt (mainCollider);
		if (!pooling) Destroy(gameObject); else gameObject.SetActive(false);
	}

	public void ReceiveOrder(Order order) 
	{
		myOrder = order;
	}

	private void MoveTo(Vector3 point) 
	{
		Vector3 goalDirection = point - transform.position;
		float distance = goalDirection.magnitude;
		float angle = Vector3.Angle(transform.forward,goalDirection);
		if (distance > contactRadius + acceleration)
		{
			if (angle < MOVEMENT_ANGLE_LIMIT && speed*speed/2/acceleration<distance-contactRadius)  speedGoal = maxSpeed;  else speedGoal = 0;
		if (angle > ANGLE_THRESHOLD) 	rotateTo = Quaternion.LookRotation(goalDirection,transform.TransformDirection(Vector3.up));
	    }
		else speedGoal = 0;
	}
		
		
}
