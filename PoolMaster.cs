using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolMaster : MonoBehaviour
	{
	const int WRECKS_MAX_COUNT = 50;

	GameObject wreckPrefab;
	ParticleSystem explosionEmitter;

	List<Destructible> wrecks;

	void Awake() 
	{
		GameMaster.pool = this;
		explosionEmitter = Instantiate (Resources.Load<ParticleSystem>("Prefs/explosion")) as ParticleSystem;
		wreckPrefab = Instantiate (Resources.Load<GameObject>("Prefs/wreckBlock")) as GameObject;
	}

	public void ExplosionAt(Vector3 position, int size) 
	{
		explosionEmitter.transform.position = position;
		explosionEmitter.Emit(size);
	}
		
	public void DestructionAt (BoxCollider collider) 
	{
		float mgn = collider.size.magnitude;
		ExplosionAt(collider.transform.position, (int)(mgn));

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
				    wreck.name = collider.gameObject.name + "_wreck";
					d = 	wreck.AddComponent <Destructible>();
					wrecks.Add (d);
				}
				else 
				{
					d = wrecks[0];
				}
			}


		d.transform.position = collider.transform.position;
		d.transform.rotation = collider.transform.rotation;
		d.transform.localScale = collider.size;
		d.SetData (mgn, 0, 0, mgn, true);
		}

	}


