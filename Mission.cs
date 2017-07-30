using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour {
	// Use this for initialization
	void Awake () {
		if (gameObject.GetComponent<Storage>() == null) GameMaster.storage = gameObject.AddComponent<Storage>();
		if (gameObject.GetComponent<PoolMaster>() == null) GameMaster.pool = gameObject.AddComponent<PoolMaster>();
	}
	
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
	}
}
