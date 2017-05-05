using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	const float MIN_ANGLE_FOR_FIRE = 1;

	public Transform[] guns;
	public MaterialPurpose raysMaterialType;
	public SpritePurpose startSpriteType;
	public float maxDistance;
	public float maxAngle;
	public float reloadTime;
	public float damagePerSecond;
	public int penetration;
	public float pointingSpeed;
	public float shotTime;

	bool firing = false;
	bool ready = true;
	bool allGunsPrepared = true;
	float firingTimeLeft = 0;
	float reloadingTimeLeft = 0;
	Destructible target;
	Controller myController;

	Vector3[] startDirections;
	LineRenderer[] rays;
	GameObject[] startSplashes;
	GameObject[] endSplashes;

	void Awake() 
	{
		int count = guns.Length;
		if (count == 0) Destroy(this);
		else {
			startDirections = new Vector3[count];
			rays = new LineRenderer[count];
			startSplashes = new GameObject[count];
			endSplashes = new GameObject[count];

			Material raysMaterial = GameMaster.storage.GetMaterial(raysMaterialType);
			Sprite startSprite = GameMaster.storage.GetSprite(startSpriteType);

			for (int i = 0; i < count; i++)
			{
				startDirections[i] = transform.InverseTransformDirection(guns[i].transform.forward);

				rays[i] = guns[i].gameObject.AddComponent<LineRenderer>();
				rays[i].material = raysMaterial;
				rays[i].startWidth = damagePerSecond/15;
				rays[i].numPositions = 2;
				rays[i].receiveShadows = false;
				rays[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				startSplashes[i] =new GameObject("startSplash");
				startSplashes[i].transform.parent = guns[i].transform;
				startSplashes[i].transform.localPosition = Vector3.zero;
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

		myController  = transform.root.gameObject.GetComponent<Controller>();

	}

	void Update() 
	{
		if (GameMaster.pause) return;

		if (target != null) 
		{
			if (!target.gameObject.activeSelf) {target = null; firing = false;}
		}

		float t = Time.deltaTime;
		int i;

		if (firing) 
		{
			if (firingTimeLeft > 0)
			{
				firingTimeLeft -= t;
				if (firingTimeLeft <=0) {firing = false;target = null;}
			}

			RaycastHit rh;
			for (i = 0; i < guns.Length; i++)
			{				
				rays[i].SetPosition(0, guns[i].position);

				Destructible d = null;
				if (Physics.Raycast(guns[i].position, guns[i].forward, out rh, maxDistance))
				{
					d	= rh.collider.GetComponent<Destructible>();
				}
	
				if (target == null) GunToStartPos(i,t);
				else 
				{
					if (d == null || d != target) PointGunOnObject(i, target.transform, t);
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
				for (i = 0; i< guns.Length; i++)
				{
					PointGunOnObject(i, target.transform, t);
				}
				if (Vector3.Angle (guns[guns.Length/2].forward, target.transform.position - guns[guns.Length/2].position) < MIN_ANGLE_FOR_FIRE)
				{
					ActivateGuns();
				}
			}
			else
			{  //GUNS AFTER FIRE
				if (reloadingTimeLeft <=0) // for one-shot events where shot time is zero
				{
					for (i = 0; i < guns.Length; i++)
					{
						rays[i].enabled = false;
						startSplashes[i].SetActive(false);
						endSplashes[i].SetActive(false);
					}
					reloadingTimeLeft = reloadTime;
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
					int c = 0;
					for (i = 0; i< guns.Length; i++) 
					{
						if (GunToStartPos(i, t) == true) c++;
					}
					if (c == guns.Length) allGunsPrepared = true;
				}
			}
		}

	}

	public void Fire (Destructible t) 
	{
		if (!ready) return;

		if (t != null) 
		{
			Vector3 inpos = guns[guns.Length/2].InverseTransformPoint (t.transform.position);
			if (inpos.z > 0 && Vector3.Angle(Vector3.forward, inpos) < maxAngle) target = t;
			return;
		}
		else 	ActivateGuns();
	}

	private bool PointGunOnObject (int i, Transform obj, float time)
	{
		Vector3 toPoint = Vector3.MoveTowards(guns[i].forward, obj.position - guns[i].position, pointingSpeed*time);
		if (Vector3.Angle (transform.TransformDirection(startDirections[i]), toPoint) < maxAngle) guns[i].forward = toPoint;
		if (Vector3.Angle (guns[i].forward, obj.position - guns[i].position) < MIN_ANGLE_FOR_FIRE) return true; else return false;
	}

	private bool GunToStartPos (int i, float speed)
	{
		if (guns[i].forward == transform.TransformDirection(startDirections[i])) return true;
		else {
			guns[i].forward = Vector3.MoveTowards(guns[i].forward, transform.TransformDirection(startDirections[i]), speed);
			return false;
		}
	}

	public bool IsReady () {return ready;}

	public  void ActivateGuns() 
	{
		firingTimeLeft = shotTime;
		Vector3 endPos;
		for (int i =0; i < guns.Length; i++)
		{
			rays[i].SetPosition(0, guns[i].position);
			RaycastHit rh;
			if (Physics.Raycast(guns[i].transform.position, guns[i].transform.forward, out rh, maxDistance)) {
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
		
}
