using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolMaster : MonoBehaviour
	{
	const int WRECKS_MAX_COUNT = 200;

	public ParticleSystem explosionPrefab;
	public GameObject wreckPrefab;
	ParticleSystem explosionEmitter;

	List<Destructible> wrecks;

	void Awake() 
	{
		GameMaster.pool = this;
		explosionEmitter = Instantiate (explosionPrefab) as ParticleSystem;
	}

	public void ExplosionAt(Vector3 position, int size) 
	{
		explosionEmitter.transform.position = position;
		explosionEmitter.Emit(size);
	}
		
	public void DestructionAt (BoxCollider collider) 
	{
		float mgn = collider.size.magnitude;
		return; //TESTMODE
		explosionEmitter.transform.position = collider.transform.position;
		explosionEmitter.Emit((int)(mgn));

		if (mgn < 5) return;

		Destructible d = null;
		if (wrecks != null) {
			foreach (Destructible f in wrecks)
			{
				if (!f.gameObject.activeSelf) {d = f; break;}
			}
		}
		else wrecks = new List<Destructible>();

			if (d==null)	{
				if (wrecks.Count < WRECKS_MAX_COUNT)
				{
					wrecks = new List<Destructible>();
					GameObject wreck = Instantiate (wreckPrefab, collider.transform.position, collider.transform.rotation) as GameObject;
					wreck.transform.localScale = collider.size;
					d = 	wreck.AddComponent <Destructible>();
					wrecks.Add (d);
				}
				else 
				{
					d = wrecks[0];
				}
			}

		d.SetData (mgn, 0, 0, mgn, true);
		}

	}


