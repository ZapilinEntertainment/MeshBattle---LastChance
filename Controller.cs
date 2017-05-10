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
		enemies = new List<Transform>();
	}

	void Update () {
		if (GameMaster.pause) return;

		if (periodicScan && myFleetCommand != null)
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
		if (w.weaponType == WeaponType.Autogun) periodicScan = true;
	}

	public Destructible GetEnemy(Weapon w)
	{
		if (enemies == null || enemies.Count == 0) return null;
		Vector3 weaponPosition = w.guns[w.centralPos].position;
		Vector3 weaponDirection = transform.TransformDirection(w.weaponDirection);
		float dist = 0;
		float angle = 0;
		float minDist = w.maxDistance;
		float minAngle = 180;
		Destructible target = null;
			foreach (Transform t in enemies) 
			{
			angle = Vector3.Angle( weaponDirection, t.position - weaponPosition );
			dist = Vector3.Distance (t.position, weaponPosition);
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
		return target;
	}

	public void SetFleetCommand(FleetCommand fc) 
	{
		myFleetCommand = fc;
	}
}
