using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : Controller {
	
	const float TARGET_UPDATE_TICK = 10;
	const float DECISION_TICK = 5;
	float targUpdate;
	float decTime;
	public Transform mainTarget;


	void Update () {
		if (GameMaster.IsPaused()) return;

		if (myFleetCommand != null) {
			if (maxRange != 0 )
			{
				t -= Time.deltaTime;
				if (t <= 0) {
					t = SCAN_TICK;
					enemies = myFleetCommand.GetEnemiesInRadius(transform.position, maxRange);
				}
			}

			targUpdate -= Time.deltaTime;
			if (targUpdate <= 0) {
				targUpdate = TARGET_UPDATE_TICK;
				Destructible d = myFleetCommand.GetEnemy(transform, scanRadius,true);
				if (d != null) mainTarget = d.transform;
			}
		}


			if (mainTarget != null) {
				float dist = Vector3.Distance(transform.position, mainTarget.position);
			if (dist > minRange)  unitScript.MoveTo(mainTarget.position);
			}

	}

	public override void AddWeapon(Weapon w) //copy in Controller
	{
		if (weapons == null)  {
			weapons = new List<Weapon>();
		}
		weapons.Add(w);
		//print ("new weapon added");
		float r = w.maxDistance;
		if (r < minRange) minRange = r;
		if (r > maxRange) maxRange = r;
		w.SetAuthomatic(true);
	}
}
