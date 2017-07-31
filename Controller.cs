using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
	protected const float SCAN_TICK = 1;

	public List<Transform> enemies;
	public float scanRadius = 10000;
	public float maxRange, minRange; //максимальная и минимальная дальность атаки
	public WeaponType mainWeaponType;

	protected float t;
	protected FleetCommand myFleetCommand;
	protected Unit unitScript;
	protected List<Weapon> weapons;


	void Awake () {
		minRange = scanRadius; maxRange = 0;
		enemies = new List<Transform>();
		unitScript = GetComponent<Unit>();
	}
		

	void Update () {
		if (GameMaster.IsPaused()) return;

		if (maxRange!= 0 && myFleetCommand != null)
		{
			t -= Time.deltaTime;
			if (t <= 0) {
				t = SCAN_TICK;
				enemies = myFleetCommand.GetEnemiesInRadius(transform.position, maxRange);
			}
		}
	}

	public virtual void AddWeapon(Weapon w) //copy in BotController
	{
		if (weapons == null)  {
			weapons = new List<Weapon>();
		}
		weapons.Add(w);
		float r = w.maxDistance;
		if (r < minRange) minRange = r;
		if (r > maxRange) maxRange = r;
	}

	public Destructible GetEnemy(Weapon w)
	{
		//print ("target request");
		if (enemies == null || enemies.Count == 0) return null;
		Vector3 weaponPosition = w.transform.position;
		Vector3 weaponDirection = transform.TransformDirection(w.weaponDirection);
		float dist = 0;
		float angle = 0;
		float minDist = w.maxDistance;
		float minAngle = 180;
		Destructible target = null;
			foreach (Transform t in enemies) 
			{
			if ( !w.InRange(t.position) ) continue;
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
		gameObject.tag = "Command" + fc.GetNumber().ToString();
	}
		
}
