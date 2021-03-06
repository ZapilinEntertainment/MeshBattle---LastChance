﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class GameMaster {
	static bool pause;
	public static bool spawn;
	public static float mapRadius = 25000;
	public static int frigatesLimit = 100;
	public static int weaponsTypeCount = 3;
	public static int guiCell = 32;

	public static Camera cam;
	public static GameObject playerShip;
	public static PoolMaster pool;
	public static Storage storage;
	public static FleetCommand[] fcommands;
	static int fnumber = 0;

	public static int AddFleetCommand(FleetCommand fc) 
	{
		if (fcommands == null) fcommands = new FleetCommand[4];
		fcommands[fnumber] = fc;
		fnumber++;
		return (fnumber);
	}

	public static int GetFleetsCount() {return fnumber;}

	public static bool IsPaused() {return pause;}
	public static void SetPause (bool x) {
		if (x) {pause = true; Time.timeScale = 1;}
		else {pause = false; Time.timeScale = 0;}
	}
	public static void SetPause () {
		if (!pause) {pause = true; Time.timeScale = 1;}
		else {pause = false; Time.timeScale = 0;}
	}
}
