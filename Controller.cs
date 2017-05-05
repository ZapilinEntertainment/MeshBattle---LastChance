using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	const float SCAN_TICK = 1;

	public List<Transform> enemies;
	public float scanRadius = 300;
	public bool periodicScan = false;

	float t;
	FleetCommand myFleetCommand;
	List<Weapon> weapons;
	void Awake () {
		
	}

	void Update () {
		if (periodicScan)
		{
			t -= Time.deltaTime;
			if (t <= 0) {
				t = SCAN_TICK;
				enemies = myFleetCommand.GetEnemiesInRadius(transform.position, scanRadius);
			}
		}
	}

	public void AddWeapon(Weapon w) 
	{
		if (weapons == null)  {
			weapons = new List<Weapon>();
		}
		weapons.Add(w);
	}
}
