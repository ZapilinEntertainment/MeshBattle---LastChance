using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBehaviour : MonoBehaviour {
	public Transform gates1, gates2;
	public GameObject frigatePref;

	void Start () {
		GameObject g = new GameObject("fleetCommand0");
		FleetCommand fc = g.AddComponent<FleetCommand>();
		fc.SetNumber(0);
		fc.gates = gates1;
		fc.frigatePrefab = frigatePref;
		GameMaster.AddFleetCommand(fc);
		fc = g.AddComponent<FleetCommand>();
		fc.SetNumber(1);
		fc.gates = gates2;
		fc.frigatePrefab = frigatePref;
		GameMaster.AddFleetCommand(fc);
	}
	

}
