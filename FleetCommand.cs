using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetCommand : MonoBehaviour {
	const float SCAN_TICK = 1;

	int number;
	List<Transform> enemies;
	float t;

	void Awake () {
		number = GameMaster.AddFleetCommand(this);
	}

	void Update() 
	{
		t -= Time.deltaTime;
		if (t <= 0) {Scan(); t= SCAN_TICK;}
		foreach (Transform t in enemies) 
		{
			if (t == null) enemies.Remove(t);
		}
	}

	public List<Transform> GetEnemiesInRadius(Vector3 pos, float radius) 
	{
		List<Transform> myEnemies = new List<Transform>();
		foreach (Transform t in enemies) {
			if (Vector3.Distance(t.position, pos) <= radius) myEnemies.Add(t);
		}
		if (myEnemies.Count !=0) return myEnemies; else return null;
	}

	public Destructible GetEnemy(Transform ship, float radius, bool useSideGuns)
	{
		float dist = 0;
		float angle = 0;
		float minDist = radius;
		float minAngle = 180;
		Destructible target = null;
		Vector3 shipAttackVector;
		if (useSideGuns) {
			shipAttackVector = ship.TransformDirection(Vector3.right);
			foreach (Transform t in enemies) 
			{
				angle = Vector3.Angle(shipAttackVector, t.position - ship.position );
				if (angle >= 180) angle -= 90;
				dist = Vector3.Distance (ship.position, t.position);
				if (angle < minAngle) 
				{
					minAngle = angle;
					target = t.GetComponent<Destructible>();
					minDist = dist;
				}
				else {
					if (angle == minAngle) {
						if (dist < minDist) 
						{
							minAngle = angle;
							target = t.GetComponent<Destructible>();
							minDist = dist;
						}
					}
				}
			}
		}
		else {//forward-oriented guns
			shipAttackVector = ship.TransformDirection(Vector3.forward);
			foreach (Transform t in enemies) 
			{
				angle = Vector3.Angle(shipAttackVector, t.position - ship.position );
				dist = Vector3.Distance (ship.position, t.position);
				if (angle < minAngle) 
				{
					minAngle = angle;
					target = t.GetComponent<Destructible>();
					minDist = dist;
				}
				else {
					if (angle == minAngle) {
						if (dist < minDist) 
						{
							minAngle = angle;
							target = t.GetComponent<Destructible>();
							minDist = dist;
						}
					}
				}
			}
		}
		return target;
	}

	protected void Scan() 
	{
		enemies = new List<Transform>();
		for (int i = 0; i< 4; i++)
		{
			if (i == number) continue;
			GameObject[] go = GameObject.FindGameObjectsWithTag("Command"+i.ToString());
			foreach (GameObject g in go)
			{
				if (g.activeSelf == true && g.GetComponent<Destructible>() != null) 
				{
					enemies.Add( g.transform );
				}
			}
		}
	}
}
