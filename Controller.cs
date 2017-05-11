using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	const float SCAN_TICK = 1;
	const float TARGET_UPDATE_TICK = 10;

	public List<Transform> enemies;
	public float scanRadius = 0;
	WeaponType mainWeaponType;
	float[] range, totalDPS;
	public bool periodicScan = false;

	float t, targUpdate;
	FleetCommand myFleetCommand;
	Transform mainTarget;
	Unit unitScript;
	List<Weapon> weapons;


	void Awake () {
		enemies = new List<Transform>();
		range = new float[GameMaster.weaponsTypeCount];
		for (int i = 0; i < range.Length; i++) range[i]=10000;
		totalDPS = new float[range.Length];
		unitScript = GetComponent<Unit>();
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

		if (myFleetCommand != null) {
			targUpdate -= Time.deltaTime;
			if (targUpdate <= 0) {
				targUpdate = TARGET_UPDATE_TICK;
				Destructible d = myFleetCommand.GetEnemy(transform, range[(int)mainWeaponType],true);
				if (d != null) mainTarget = d.transform;
			}
		}

		if (mainTarget !=null) {
			float dist = Vector3.Distance(transform.position, mainTarget.position);
			if (dist < range[(int)mainWeaponType]) unitScript.RotateTo(Quaternion.FromToRotation(transform.forward, mainTarget.forward));
			else unitScript.MoveTo(mainTarget.position);
		}
	}

	public void AddWeapon(Weapon w) 
	{
		if (weapons == null)  {
			weapons = new List<Weapon>();
		}
		weapons.Add(w);
		if (w.maxDistance < range[(int)w.weaponType]) range[(int)w.weaponType] = w.maxDistance;
		totalDPS[(int)w.weaponType] += w.damagePerSecond * w.guns.Length;

		float max = 0;
		for (int i =0 ; i < totalDPS.Length; i++) {
			if (totalDPS[i] > max) 
			{
				max = totalDPS[i];
				mainWeaponType = (WeaponType)i;
			}
		}
		if (w.weaponType == WeaponType.Autogun) periodicScan = true;
		GetComponent<Unit>().speedGoal = 10;
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

	public WeaponType GetMainWeaponType() {return mainWeaponType;}

	public float GetAttackRadius() {
		return range[(int)mainWeaponType];
	}
}
