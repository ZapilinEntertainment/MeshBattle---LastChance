using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public Transform[] guns;
	public MaterialPurpose raysMaterial;
	public float maxDistance;
	public float maxAngle;
	public float reloadTime;
	public float damage;
	public int penetration;
	bool ready;
	float timeLeft = 0;

	LineRenderer[] rays;
	GameObject[] startSplashes;
	GameObject[] endSplashes;

	void Awake() 
	{
		int count = guns.Length;
		if (count == 0) Destroy(this);
		else {
			rays = new LineRenderer[count];
			startSplashes = new GameObject[count];
			endSplashes = new GameObject[count];

			for (int i = 0; i < count; i++)
			{
				rays[i].material = GameMaster.storage.GetMaterial(MaterialPurpose.laser_ray_green);
				rays[i].enabled = false;


			}
			}
	}
}
