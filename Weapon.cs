using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
	MainCaliber,
	Raycaster,
	Autogun
}

public class Weapon : MonoBehaviour {

	protected const float MIN_ANGLE_FOR_FIRE = 1;
	protected const float REFRESH_TICK = 3;

	public Transform[] guns;
	public float trunkLength = 0; 
	public MaterialPurpose raysMaterialType;
	public SpritePurpose startSpriteType;
	public WeaponType weaponType;
	public float maxDistance;
	public float maxAngle;
	public float reloadTime;
	public float damagePerSecond;
	public int penetration;
	public float pointingSpeed;
	public float shotTime;
	public Vector3 weaponDirection;
	Vector3 pointingVector;

	protected bool firing = false;
	protected bool ready = true;
	protected bool allGunsPrepared = true;
	protected bool authomatic = false;
	protected bool working = false;
	protected float firingTimeLeft = 0;
	protected float reloadingTimeLeft = 0;
	protected float updatingEnemy = 0;
	public Destructible target;
	protected Controller myController;

	protected LineRenderer[] rays;
	protected GameObject[] startSplashes;
	protected GameObject[] endSplashes;

	void Awake() 
	{
		int count = guns.Length;
		if (count == 0) 	{Destroy(this);return;}
		Calibrate();

		if (weaponType == WeaponType.Autogun) authomatic = true;
		rays = new LineRenderer[count];
		startSplashes = new GameObject[count];
		endSplashes = new GameObject[count];

		Material raysMaterial = GameMaster.storage.GetMaterial(raysMaterialType);
		Sprite startSprite = GameMaster.storage.GetSprite(startSpriteType);

		for (int i = 0; i < count; i++)
		{
				rays[i] = guns[i].gameObject.AddComponent<LineRenderer>();
				rays[i].material = raysMaterial;
				rays[i].startWidth = damagePerSecond/15;
				rays[i].numPositions = 2;
				rays[i].receiveShadows = false;
				rays[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				startSplashes[i] =new GameObject("startSplash");
				startSplashes[i].transform.parent = guns[i].transform;
				startSplashes[i].transform.localPosition = Vector3.forward * trunkLength;
				SpriteRenderer sr = startSplashes[i].AddComponent<SpriteRenderer>();
				sr.sprite = startSprite;
				SpriteController sc = startSplashes[i].AddComponent<SpriteController>();
				sc.SetData(Vector3.one * damagePerSecond /25.0f, penetration, 0);

				endSplashes[i] = new GameObject("endSplash");
				endSplashes[i].transform.parent = guns[i].transform;
				endSplashes[i].transform.localPosition = Vector3.zero;
				sr = endSplashes[i].AddComponent<SpriteRenderer>();
				sr.sprite = startSprite;
				sc = endSplashes[i].AddComponent<SpriteController>();
				sc.SetData(Vector3.one * damagePerSecond /25.0f, penetration, 0);

				rays[i].enabled = false;
				startSplashes[i].SetActive(false);
				endSplashes[i].SetActive(false);
		}
	}

	protected virtual void Calibrate() {
		if (weaponDirection == Vector3.zero) weaponDirection = transform.root.InverseTransformDirection(guns[guns.Length/2].forward);
		pointingVector = transform.root.TransformDirection(weaponDirection);
		foreach (Transform gun in guns) gun.transform.forward = pointingVector;
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
			if (firing) StopGuns();
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

			if (target != null) PointGunsOnDirection(target.transform.position - transform.position, t);
			RaycastHit rh;
			for (i = 0; i < guns.Length; i++)
			{				
				rays[i].SetPosition(0, guns[i].position + guns[i].forward * trunkLength);

				Destructible d = null;
				if (Physics.Raycast(guns[i].position + guns[i].forward * trunkLength, guns[i].forward , out rh, maxDistance))
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
		}
		else
		{
			if (target != null) //GUNS POINTING ON TARGET
			{
				if (PointGunsOnDirection(target.transform.position - transform.position, t) == true)		ActivateGuns();
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
				if (!allGunsPrepared) allGunsPrepared = PointGunsOnDirection(transform.root.TransformDirection(weaponDirection), t);
			}
		}
	}

	public void Fire (Destructible t) 
	{
		if (!ready) return;

		if (t != null) 
		{
			Vector3 inpos = transform.InverseTransformPoint (t.transform.position);
			if (inpos.z > 0 && Vector3.Angle(Vector3.forward, inpos) < maxAngle) target = t;
			return;
		}
		else 	ActivateGuns();
	}

	bool PointGunsOnDirection (Vector3 dir, float time)
	{
		pointingVector = Vector3.RotateTowards(pointingVector, dir, pointingSpeed * time, pointingVector.magnitude );
		foreach (Transform gun in guns) gun.forward = pointingVector;
		if (Vector3.Angle (pointingVector, dir) < MIN_ANGLE_FOR_FIRE) return true; else return false;
	}


	public bool IsReady () {return ready;}

	public  void ActivateGuns() ///raycasting
	{
		firingTimeLeft = shotTime;
		Vector3 endPos;
		for (int i =0; i < guns.Length; i++)
		{
			rays[i].SetPosition(0, guns[i].position  + guns[i].forward * trunkLength );
			RaycastHit rh;
			if (Physics.Raycast(guns[i].transform.position  + guns[i].forward * trunkLength, guns[i].transform.forward, out rh, maxDistance)) {
				endPos = rh.point;
			}
			else endPos = guns[i].position + guns[i].forward * maxDistance;

			rays[i].SetPosition(1, endPos);
			rays[i].endWidth = Vector3.Distance(rays[i].GetPosition(0), endPos) / maxDistance * rays[i].startWidth;
			endSplashes[i].transform.position = endPos;

			startSplashes[i].SetActive(true);
			rays[i].enabled = true;
			endSplashes[i].SetActive(true);
		}

		firingTimeLeft = shotTime;
		reloadingTimeLeft = 0;
		firing = true;
		ready = false;
		allGunsPrepared = false;
	}

	public void StopGuns() {
		target = null;
		firing = false;
		for (int i = 0; i < guns.Length; i++)
		{
			rays[i].enabled = false;
			startSplashes[i].SetActive(false);
			endSplashes[i].SetActive(false);
		}
		reloadingTimeLeft = reloadTime;
	}
		
}
