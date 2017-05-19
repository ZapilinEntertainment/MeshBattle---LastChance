using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWeapon : Weapon {
	public float maxGunsAngle;
	Vector3 gunStartDir;

	protected override void Calibrate () {
		weaponDirection = transform.root.InverseTransformDirection(transform.forward); 
		gunStartDir = transform.InverseTransformDirection(guns[0].forward);
	}

	bool PointTowerOnObject (Transform obj, float time)
	{
		Vector3 inpos = transform.InverseTransformPoint( obj.position ); 
		Quaternion rt = Quaternion.LookRotation(transform.TransformDirection(new Vector3(inpos.x, 0, inpos.z)), transform.TransformDirection(Vector3.up));
		bool prepared = false, prepared2 = false;
		if (Quaternion.Angle (transform.rotation, rt) < MIN_ANGLE_FOR_FIRE) prepared =true;
		else transform.rotation = Quaternion.RotateTowards(transform.rotation, rt, pointingSpeed * time);

		int c = 0;
		foreach (Transform gun in guns) {
			inpos = gun.InverseTransformPoint(target.transform.position); 
			rt = Quaternion.LookRotation(gun.TransformDirection(inpos));
			gun.rotation = Quaternion.RotateTowards (gun.rotation, rt, pointingSpeed * time);
			if (Quaternion.Angle (gun.rotation, rt) < MIN_ANGLE_FOR_FIRE) c++;
		}
		if (c == guns.Length) prepared2 = true;
		if (prepared  == true && prepared2 == true) return true; else return false;
	}

	bool TowerToStartPos (float time)
	{
		bool prepared = false, prepared2 = false;
		Quaternion rt = Quaternion.LookRotation(transform.root.TransformDirection(weaponDirection), transform.TransformDirection(Vector3.up)); 
		if (transform.rotation == rt) prepared = true;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rt, pointingSpeed * time);

		int c =0;
		foreach (Transform gun in guns) {
			rt = Quaternion.LookRotation(transform.TransformDirection(gunStartDir));
			gun.rotation = Quaternion.RotateTowards (gun.rotation, rt, pointingSpeed * time);
			if (Quaternion.Angle (gun.rotation, rt) < MIN_ANGLE_FOR_FIRE) c++;
		}
		if (c == guns.Length) prepared2 = true;
		if (prepared  == true && prepared2 ==true) return true; else return false;
	}

	void Update() 
	{
		if (GameMaster.pause ) return;
		if (!working) {
			myController  = transform.root.gameObject.GetComponent<Controller>();
			if (myController != null) {myController.AddWeapon(this);working = true;}
			return;
		}

		if (target != null) 
		{
			if (!target.gameObject.activeSelf) StopGuns();
		}
		else 
		{
			if (authomatic&&ready) {
				updatingEnemy -= Time.deltaTime;
				if (updatingEnemy <= 0) {
					updatingEnemy = REFRESH_TICK;
					target = myController.GetEnemy(this);
				}
			}
		}

		float t = Time.deltaTime;
		int i;

		if (firing) 
		{
			if (firingTimeLeft > 0)
			{
				firingTimeLeft -= t;
				if (firingTimeLeft <=0) StopGuns();
			}

			RaycastHit rh;
			for (i = 0; i < guns.Length; i++)
			{				
				rays[i].SetPosition(0, guns[i].position  + guns[i].forward * trunkLength);

				Destructible d = null;
				if (Physics.Raycast(guns[i].position  + guns[i].forward * trunkLength, guns[i].forward, out rh, maxDistance))
				{
					d	= rh.collider.GetComponent<Destructible>();
				}

				Vector3 endPoint;
				if (rh.collider != null) {
					endPoint = rh.point;
					rh.collider.SendMessage("ApplyDamage", new Damage(damagePerSecond*t, penetration*t, rh.point));
				}
				else endPoint= guns[i].position + guns[i].forward * maxDistance;
				rays[i].SetPosition(1, endPoint);
				rays[i].endWidth = Vector3.Distance(rays[i].GetPosition(0), endPoint) / maxDistance * rays[i].startWidth;
				endSplashes[i].transform.position = endPoint;
			}

			if (target != null) PointTowerOnObject(target.transform, t);
		}
		else
		{
			if (target != null) //GUNS POINTING ON TARGET
			{
				if (PointTowerOnObject(target.transform, t))	ActivateGuns();
			}
			else
			{  //GUNS AFTER FIRE
				if (reloadingTimeLeft <=0) // for one-shot events where shot time is zero
				{
					StopGuns();
				}
				else 
				{
					reloadingTimeLeft -= t;
					if (reloadingTimeLeft <= 0) 
					{
						ready  = true;
						target = null;
					}
				}
				if (!allGunsPrepared) 
				{
					allGunsPrepared = TowerToStartPos(t);
				}
			}
		}
	}
}
