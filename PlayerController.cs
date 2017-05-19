using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {

	public Texture frame_tx;
	int k; // guiCell
	Rect speedBox, stateBox;

	Vector3 rotatingVector = Vector3.zero;
	int speedMode = 0;

	void Awake() {
		unitScript = GetComponent<Unit>();
		k = GameMaster.guiCell;
		int sw =Screen.width; int sh = Screen.height;
		speedBox = new Rect(0,sh - 4*k, 4*k, k);
		stateBox = new Rect (0, 0, 6*k, k);
	}

	void Update () {
		if (GameMaster.pause) return;

		if (periodicScan && myFleetCommand != null)
		{
			t -= Time.deltaTime;
			if (t <= 0) {
				t = SCAN_TICK;
				enemies = myFleetCommand.GetEnemiesInRadius(transform.position, scanRadius);
			}
		}

		Vector3 prevRVector = rotatingVector;
		if (Input.GetKeyDown ("e")) {
			if (rotatingVector.z >= 0 ) rotatingVector.z = -1; 
			else rotatingVector.z = 0;
		}
		else {
			if (Input.GetKeyDown ("q")) {
				if (rotatingVector.z <=0) rotatingVector.z = 1;
				else rotatingVector.z = 0;
			}
		}
		if (Input.GetKeyDown ("a")) {
			if (rotatingVector.y >= 0) rotatingVector.y = -1;
			else rotatingVector.y= 0;
		}
		else {
			if (Input.GetKeyDown ("d")) {
				if (rotatingVector.y <= 0) rotatingVector.y = 1;
				else rotatingVector.y= 0;
			}
		}
		if (Input.GetKeyDown ("z")) {
			if (rotatingVector.x >= 0) rotatingVector.x = -1;
			else rotatingVector.x= 0;
		}
		else {
			if (Input.GetKeyDown ("c")) {
				if (rotatingVector.x <= 0) rotatingVector.x = 1;
				else rotatingVector.x= 0;
			}
		}
		if (prevRVector != rotatingVector) unitScript.SetRotateVector (rotatingVector);

		if (Input.GetKeyDown("w")) SetSpeedMode(speedMode+1);
		else if (Input.GetKeyDown("s")) SetSpeedMode(speedMode-1);


		if (Input.GetMouseButtonDown(0))
		{
			Ray r=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rh;
			if (Physics.Raycast(r,out rh) )
			{					
				Destructible d = rh.collider.GetComponent<Destructible>();
				if (d != null) {
				foreach (Weapon w in weapons) {
						if (w.weaponType == WeaponType.MainCaliber) w.Fire(d);
				}
				}
			}
		}
	}

	void SetSpeedMode(int x) {
		if (x < 0) x = 0;
		if (x > 3) x= 3;
		if (x == speedMode) return;
		speedMode = x; 
		switch (x) {
		case 0: unitScript.SetSpeed(0);break;
		case 1: unitScript.SetSpeed(0.25f);break;
		case 2: unitScript.SetSpeed(0.5f);break;
		case 3: unitScript.SetSpeed(1);break;
		}
	}

	void OnGUI () {
		GUI.Label (stateBox, "Hull - " + unitScript.GetHpPercentage().ToString() + "%");
		Rect r = stateBox; r.y += r.height;
		GUI.Label (r, "Armor - " + unitScript.GetArmorPercentage().ToString() + "%");

		if (GUI.Button(speedBox, "Полный вперед")) {SetSpeedMode(3);}
		r = speedBox; r.y+=r.height;
		if (GUI.Button(r, "Полхода вперед")) {SetSpeedMode(2);} r.y += r.height;
		if (GUI.Button(r, "Четверть хода ")) {SetSpeedMode(1);} r.y += r.height;
		if (GUI.Button(r, "Стоп машина")) SetSpeedMode(0);
		r.y -= k * speedMode;
		GUI.DrawTexture(r, frame_tx,ScaleMode.StretchToFill);
	}
}
