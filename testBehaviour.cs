using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testBehaviour : MonoBehaviour {
	public Transform[] gatesCommand1, gatesCommand2;
	public GameObject frigatePref;
	public Controller playerShip;
	public bool spawn = true;
	public int frigatesLimit = 100;

	void Awake() {
		GameMaster.frigatesLimit = frigatesLimit;
	}

	void Start () {

		GameObject g = new GameObject("fleetCommand1");
		FleetCommand fc = g.AddComponent<FleetCommand>();
		fc.SetNumber(1);
		fc.gates = gatesCommand1;
		fc.frigatePrefab = frigatePref;
		GameMaster.AddFleetCommand(fc);

		g = new GameObject("fleetCommand2");
		fc = g.AddComponent<FleetCommand>();
		fc.SetNumber(2);
		fc.gates = gatesCommand2;
		fc.frigatePrefab = frigatePref;
		GameMaster.AddFleetCommand(fc);
	}
	
	void Update () {
		if (GameMaster.spawn != spawn) GameMaster.spawn = spawn;
	}
}
