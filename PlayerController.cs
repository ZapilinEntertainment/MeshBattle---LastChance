using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {

	public Camera myCam;
	float angleBorder = 90;
	public float camRotationSpeed = 300;
	bool camFixed = false, viewWasChanged = false;
	float camSmoothCoefficient = 0.1f;
	public float camSmoothAcceleration = 0.7f;

	Texture frame_tx, maneuverPanel_tx;
	Texture yawPosAct_tx, yawNegAct_tx, rollPosAct_tx, rollNegAct_tx, pitchPosAct_tx, pitchNegAct_tx;
	Texture yawPos_tx, yawNeg_tx, rollPos_tx, rollNeg_tx, pitchPos_tx, pitchNeg_tx;
	int k,sw,sh; // guiCell

	Rect speedBox, stateBox, upperManeuverPanel;
	GUISkin defSkin, nbSkin;
	Rect pitchUpButtonRect, pitchDownButtonRect, rollRightButtonRect, rollLeftButtonRect, yawRightButtonRect, yawLeftButtonRect;

	Vector3 rotatingVector = Vector3.zero; // Yaw - рысканье (Y), Roll - крен (Z), Pitch - тангаж (X)
	int speedMode = 0;
	Collider underCursor;

	void Awake() {
		unitScript = GetComponent<Unit>();
		k = GameMaster.guiCell;
		sw =Screen.width; sh = Screen.height;
		speedBox = new Rect(0,sh - 4*k, 4*k, k);
		stateBox = new Rect (0, 0, 6*k, k);
		pitchUpButtonRect = new Rect(sw/2 + 3*k, 0, k, k); pitchDownButtonRect = new Rect(sw/2 - 4*k, 0, k, k);
		yawRightButtonRect = new Rect(sw/2 + 2*k, 0, k, k);yawLeftButtonRect = new Rect (sw/2 - 3*k, 0, k,k);
		rollRightButtonRect = new Rect (sw/2 + k, 0, k, k); rollLeftButtonRect = new Rect(sw/2 - 2*k, 0, k, k);

		frame_tx = Resources.Load<Texture>("GUI/frame");
		yawPos_tx = Resources.Load<Texture>("GUI/yawPos"); yawPosAct_tx = Resources.Load<Texture>("GUI/yawPosAct"); 
		yawNeg_tx= Resources.Load<Texture>("GUI/yawNeg"); yawNegAct_tx = Resources.Load<Texture>("GUI/yawNegAct");
		rollPos_tx= Resources.Load<Texture>("GUI/rollPos"); rollPosAct_tx = Resources.Load<Texture>("GUI/rollPosAct");
		rollNeg_tx= Resources.Load<Texture>("GUI/rollNeg"); rollNegAct_tx = Resources.Load<Texture>("GUI/rollNegAct");
		pitchPos_tx= Resources.Load<Texture>("GUI/pitchPos"); pitchPosAct_tx = Resources.Load<Texture>("GUI/pitchPosAct");
		pitchNeg_tx= Resources.Load<Texture>("GUI/pitchNeg"); pitchNegAct_tx = Resources.Load<Texture>("GUI/pitchNegAct");

		if (myCam == null) 
		 {
			GameObject g = transform.Find("cameraPoint").gameObject;
			myCam = g.AddComponent<Camera>();
			myCam.farClipPlane = 10000;
			g.AddComponent<GUILayer>();
			g.AddComponent<AudioListener>();
			g.AddComponent<FlareLayer>();
		}
		GameMaster.cam = myCam;
		GameMaster.playerShip = gameObject;
	}

	void Update () {
		if (GameMaster.IsPaused()) return;

		if (maxRange != 0 && myFleetCommand != null)
		{
			t -= Time.deltaTime;
			if (t <= 0) {
				t = SCAN_TICK;
				enemies = myFleetCommand.GetEnemiesInRadius(transform.position, scanRadius);
			}
		}

		Ray r=myCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit rh;
		if (Physics.Raycast(r,out rh) && rh.collider.gameObject != gameObject)
		{					
			underCursor = rh.collider;
		}
		else underCursor = null;

		if (Input.GetMouseButtonDown(0))
		{
			if (underCursor != null ) {
				Destructible d = underCursor.GetComponent<Destructible>();
				if (d != null) {
					foreach (Weapon w in weapons) {
						if (w.weaponType == WeaponType.MainCaliber) w.Fire(d);
					}
				}
			}
		}
		rotatingVector = unitScript.GetRotateVector();
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

		if (Input.GetKeyDown("f")) camFixed = !camFixed;
		if (!camFixed) {
			float delta = Input.GetAxis("Mouse X");
			float crSpeed = camRotationSpeed * Time.deltaTime * camSmoothCoefficient;
			bool newChanges = false;
			if (delta != 0) {
				myCam.transform.RotateAround(myCam.transform.position, transform.TransformDirection(Vector3.up) , crSpeed * delta);
				if (viewWasChanged) camSmoothCoefficient += camSmoothAcceleration;
				newChanges = true;
			}
			delta = Input.GetAxis ("Mouse Y") * (-1);
			if (delta != 0) {
				myCam.transform.Rotate(Vector3.right * crSpeed * delta, Space.Self);
				if (viewWasChanged) camSmoothCoefficient += camSmoothAcceleration;
				newChanges = true;
			}
			if (!newChanges) {camSmoothCoefficient = 0.1f;viewWasChanged = false;}
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

		Vector2 mpos = Input.mousePosition; mpos.y = sh - mpos.y;
		if (underCursor != null) {
			GUI.color = Color.blue;
			float dist = Vector3.Distance(transform.position, underCursor.transform.position);
			GUI.Label (new Rect (mpos.x + 2*k, mpos.y - k/2,  2*k, k), Mathf.Round(dist).ToString() + "u");
			GUI.color = Color.white;
		}
			
		if (rotatingVector.x != 0) {
			if (rotatingVector.x > 0) {
				if (GUI.Button(pitchDownButtonRect, pitchPosAct_tx)) unitScript.SetXRotation(0);
				if (GUI.Button(pitchUpButtonRect, pitchNeg_tx)) unitScript.SetXRotation(-1);
			}
			else {
				if (GUI.Button(pitchUpButtonRect, pitchNegAct_tx)) unitScript.SetXRotation(0);
				if (GUI.Button(pitchDownButtonRect, pitchPos_tx)) unitScript.SetXRotation(1);
			}
		}
		else {
			if (GUI.Button(pitchUpButtonRect, pitchNeg_tx)) unitScript.SetXRotation(-1);
			if (GUI.Button(pitchDownButtonRect, pitchPos_tx)) unitScript.SetXRotation(1);
		}

		if (rotatingVector.y != 0) {
			if (rotatingVector.y > 0) {
				if (GUI.Button(yawRightButtonRect, yawPosAct_tx)) unitScript.SetYRotation(0);
				if (GUI.Button(yawLeftButtonRect, yawNeg_tx)) unitScript.SetYRotation(-1);
			}
			else 
			{
				if (GUI.Button(yawRightButtonRect, yawPos_tx)) unitScript.SetYRotation(1);
				if (GUI.Button(yawLeftButtonRect, yawNegAct_tx)) unitScript.SetYRotation(0);
			}
		}
		else 
		{
			if (GUI.Button(yawRightButtonRect, yawPos_tx)) unitScript.SetYRotation(1);
			if (GUI.Button(yawLeftButtonRect, yawNeg_tx)) unitScript.SetYRotation(-1);
		}

		if (rotatingVector.z != 0) {
			if (rotatingVector.z < 0) 
			{
				if (GUI.Button(rollRightButtonRect, rollNegAct_tx)) unitScript.SetZRotation(0);
				if (GUI.Button(rollLeftButtonRect, rollPos_tx)) unitScript.SetZRotation(1);
			}
			else 
			{
				if (GUI.Button(rollRightButtonRect, rollNeg_tx)) unitScript.SetZRotation(-1);
				if (GUI.Button(rollLeftButtonRect, rollPosAct_tx)) unitScript.SetZRotation(0);
			}
		}
		else 
		{
			if (GUI.Button(rollRightButtonRect, rollNeg_tx)) unitScript.SetZRotation(-1);
			if (GUI.Button(rollLeftButtonRect, rollPos_tx)) unitScript.SetZRotation(1);
		}

		if (GUI.Button(speedBox, "Полный вперед")) {SetSpeedMode(3);}
		r = speedBox; r.y+=r.height;
		if (GUI.Button(r, "Полхода вперед")) {SetSpeedMode(2);} r.y += r.height;
		if (GUI.Button(r, "Четверть хода ")) {SetSpeedMode(1);} r.y += r.height;
		if (GUI.Button(r, "Стоп машина")) SetSpeedMode(0);
		r.y -= k * speedMode;
		GUI.DrawTexture(r, frame_tx,ScaleMode.StretchToFill);

		GUI.Label (new Rect(sw - 4*k, 0, 4*k, 2*k), "F для фиксации камеры");
	}
		
}
