using System;
using UnityEngine;

public class Damage
	{
		public float damage;
		public float penetration;
		public Vector3 point;

		public Damage (float dmg, float pnt, Vector3 place)
		{
			if (dmg < 0) damage= 0; else damage = dmg;
			if (pnt < 0) penetration = 0; else penetration = pnt;
			point = place;
		}
	}


