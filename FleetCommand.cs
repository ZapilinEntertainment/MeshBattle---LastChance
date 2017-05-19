using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetCommand : MonoBehaviour {
	const float SCAN_TICK = 1;
	const float SPAWN_RATIO = 1;
	const float GATES_RADIUS = 100;

	int number;
	int shipsCount = 0;
	int lastForcedFrigate = 0;
	GameObject[] frigates;
	public GameObject frigatePrefab;
	List<Transform> enemies;
	public Transform gates;
	float t , tspawn;

	void Awake() {
		frigates = new GameObject[GameMaster.frigatesLimit];
	}

	void Update() 
	{
		if (gates != null) {
		tspawn -= Time.deltaTime;
		if (tspawn <= 0) {SpawnFrigate(); tspawn = SPAWN_RATIO;}
		}

		t -= Time.deltaTime;
		if (t <= 0) {Scan(); t= SCAN_TICK;}
	}

	public List<Transform> GetEnemiesInRadius(Vector3 pos, float radius) 
	{
		List<Transform> myEnemies = new List<Transform>();
		foreach (Transform t in enemies) {
			if (Vector3.Distance(t.position, pos) <= radius) myEnemies.Add(t);
		}
		if (myEnemies.Count !=0) return myEnemies; else return null;
	}

	public Destructible GetEnemy(Transform ship, float radius, bool useSideGuns)
	{
		float dist = 0;
		float angle = 0;
		float minDist = radius;
		float minAngle = 180;
		Destructible target = null;
		Vector3 shipAttackVector;
		if (useSideGuns) {
			shipAttackVector = ship.TransformDirection(Vector3.right);
			foreach (Transform t in enemies) 
			{
				angle = Vector3.Angle(shipAttackVector, t.position - ship.position );
				if (angle >= 180) angle -= 90;
				dist = Vector3.Distance (ship.position, t.position);
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
		}
		else {//forward-oriented guns
			shipAttackVector = ship.TransformDirection(Vector3.forward);
			foreach (Transform t in enemies) 
			{
				angle = Vector3.Angle(shipAttackVector, t.position - ship.position );
				dist = Vector3.Distance (ship.position, t.position);
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
		}
		return target;
	}

	protected void Scan() 
	{
		if (enemies == null) enemies = new List<Transform>();
		else enemies.Clear();
		shipsCount = 0;
		for (int i = 0; i< 4; i++)
		{
			GameObject[] go = GameObject.FindGameObjectsWithTag("Command"+i.ToString());
			if (i == number ) {
				foreach (GameObject g in go)
				{
					if (g.activeSelf == true && g.GetComponent<Destructible>() != null) shipsCount++;
				}
			}
			else {
				foreach (GameObject g in go)
				{
					if (g.activeSelf == true && g.GetComponent<Destructible>() != null) 
					{
						enemies.Add( g.transform );
					}
				}
			}
		}
	}

	public void SetNumber(int x) { 
		number = x;
		gameObject.tag = "Command"+x.ToString();
	}

	void SpawnFrigate() 
	{
		if (shipsCount >= GameMaster.frigatesLimit) return;
		int trials = 5;
		float size = frigatePrefab.GetComponent<Destructible>().mainCollider.size.magnitude;
		bool spawnAccepted = false;
		Collider[] cds;
		Vector3 spawnPos = gates.position;
		for (int k = 0; k< trials; k++)
		{
			spawnPos = gates.position + Random.onUnitSphere * GATES_RADIUS;
			cds = Physics.OverlapSphere(spawnPos, size);
			if (cds.Length == 0) {spawnAccepted = true; break;}
		}
		if ( !spawnAccepted ) return;

		int foundIndex = -1;
		GameObject frigate = null;
		for (int i = 0; i < frigates.Length; i++)
		{
			if ( frigates[i] == null ) {
				foundIndex = i;
				frigate = Instantiate(frigatePrefab, spawnPos, gates.rotation) as GameObject;
				frigate.name = "c"+number.ToString()+"_frigate"+i.ToString();
				frigate.tag = "Command"+number.ToString();
				frigate.GetComponent<Controller>().SetFleetCommand(this);
				frigates[i] = frigate;			
				break;
			}
			else {
				if (frigates[i].activeSelf == false) 
				{
					foundIndex = i;
					frigate = frigates[i];
					frigate.GetComponent<Destructible>().Recreate();
					break;
				}
			}
		}

		if (foundIndex == -1) {
			frigate = frigates[lastForcedFrigate];
			frigate.GetComponent<Destructible>().Recreate();
			lastForcedFrigate++;
			if (lastForcedFrigate >= frigates.Length) lastForcedFrigate = 0;
		}
			
		frigate.transform.position = spawnPos;
		frigate.transform.rotation = gates.rotation;
		frigate.SetActive(true);
	}
}
