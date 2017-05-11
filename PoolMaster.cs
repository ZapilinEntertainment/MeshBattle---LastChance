using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolMaster : MonoBehaviour
	{
	const int WRECKS_MAX_COUNT = 500;
	const int WRECK_BLOCK_SCALE = 1;
	const float WRECK_MASS_COEFFICIENT = 0.1f;
	const float WRECK_SPAWN_CHANCE = 0.3f;
	const float WRECK_TIME = 60;

	int lastForcedWreck = 0;
	GameObject wreckPrefab;
	ParticleSystem explosionEmitter;
	Destructible[] wrecks;

	void Awake() 
	{
		GameMaster.pool = this;
		explosionEmitter = Instantiate (Resources.Load<ParticleSystem>("Prefs/explosion")) as ParticleSystem;
		wreckPrefab = Instantiate (Resources.Load<GameObject>("Prefs/wreckBlock")) as GameObject;
		wrecks = new Destructible[WRECKS_MAX_COUNT];
		wrecks[0] = wreckPrefab.GetComponent<Destructible>();
		wreckPrefab.SetActive(false);
	}

	public void ExplosionAt(Vector3 position, int size) 
	{
		explosionEmitter.transform.position = position;
		explosionEmitter.Emit(size);
	}

	public void PiecesAt (Vector3 position, int size) {}
		
	public void DestructionAt (BoxCollider collider, Vector3 speed) 
	{
		Vector3 pos = collider.transform.position;
		float mgn = collider.size.magnitude;
		int a = (int)collider.size.x;
		int b = (int)collider.size.y;
		int c = (int)collider.size.z;
		ExplosionAt(pos, (int)(mgn));

		if (a < WRECK_BLOCK_SCALE || b < WRECK_BLOCK_SCALE || c < WRECK_BLOCK_SCALE) {PiecesAt(pos, (int)mgn);return;}

		int i,j,k;
		for (i  = 0; i < a; i++) {
			for ( j = 0 ; j< b; j++) {
				for (k = 0; k<c;k++) {
					if (UnityEngine.Random.value < WRECK_SPAWN_CHANCE) WreckAt(pos + new Vector3(i - a/2, j - b/2, k - c/2) * WRECK_BLOCK_SCALE, speed) ;
				}
			}
		}
			
		}

	private void WreckAt( Vector3 pos, Vector3 vel) {
		int searchedIndex = -1;
		Destructible d = null;
		for (int i = 0; i < wrecks.Length; i++)
		{
			if (wrecks[i] == null) {
				GameObject wreck = Instantiate (wreckPrefab, pos, Quaternion.identity) as GameObject;
				wreck.name = "wreck"+i.ToString();
				wreck.transform.localScale = Vector3.one * WRECK_BLOCK_SCALE;
				wreck.GetComponent<Rigidbody>().velocity = UnityEngine.Random.onUnitSphere + vel;
				 d = 	wreck.GetComponent <Destructible>();
				d.UseTimer(WRECK_TIME+ UnityEngine.Random.value*5);
				wrecks[i] = d;
				d.SetPooling(true);
				searchedIndex = i;
				break;
			}
			else
			{
				if ( !wrecks[i].gameObject.activeSelf ) 
				{
					 d = wrecks[i];
					d.Recreate();
					searchedIndex = i;
					break;
				}
			}
		}
		if ( searchedIndex == -1) 
		{
			d = wrecks[lastForcedWreck];
			searchedIndex = lastForcedWreck;
			lastForcedWreck++;
			if (lastForcedWreck >= wrecks.Length) lastForcedWreck = 0;
			d.Recreate();
		}
		wrecks[searchedIndex].transform.parent = null;
		wrecks[searchedIndex].transform.position = pos;
	}
	}


