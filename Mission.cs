using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class Mission : MonoBehaviour {
	int sw,sh,k;
	// Use this for initialization
	void Awake () {
		if (gameObject.GetComponent<Storage>() == null) GameMaster.storage = gameObject.AddComponent<Storage>();
		if (gameObject.GetComponent<PoolMaster>() == null) GameMaster.pool = gameObject.AddComponent<PoolMaster>();
	}

	void Start() {sw = Screen.width; sh = Screen.height; k = GameMaster.guiCell;}

	// Update is called once per frame
	void Update () {
		if (GameMaster.cam == null && GameMaster.playerShip == null) 
		{
			GameObject g = new GameObject ("reserved camera");
			Camera cam = g.AddComponent<Camera>();
			cam.farClipPlane = 10000;
			g.AddComponent<GUILayer>();
			g.AddComponent<FlareLayer>();
			g.AddComponent<AudioListener>();
			GameMaster.cam = cam;
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			sw = Screen.width;
			sh= Screen.height;
			GameMaster.SetPause();
		}
	}

	void OnGUI() {
		if (GameMaster.IsPaused()) {
			if (GUI.Button(new Rect(sw/2 - k, sh/2 - 2*k, 4*k, 2*k), "Продолжить")) { GameMaster.SetPause(false);}
			if (GUI.Button(new Rect(sw/2 - k, sh/2, 4*k, 2*k), "Выйти")) { Application.Quit();}
		}
		else {
			if (GUI.Button(new Rect(sw - 2*k, 0, 2*k, k), "Выход")) Application.Quit();
		}
	}
}
