using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class labirynthMission : Mission {
	public float minRadius = 50, maxRadius = 500;
	public float minRange = 350, maxRange = 1200; 
	public int minCount = 3, maxCount = 25;
	public GameObject spherePref;

	// Use this for initialization
	void Start () {
		float radius1 = (maxRange - minRange) * Random.value + minRange;
		int count = (int)(minCount + (maxCount - minCount) * Random.value);
		Vector3 point = Vector3.zero;
		float spRadius = 0;
		GameObject g;
		for (int i =0; i< count; i++) {
			point = Random.onUnitSphere * radius1;
			spRadius = (maxRadius - minRadius) * Random.value + minRadius;
			g = Instantiate (spherePref, point, Quaternion.identity) as GameObject;
			g.transform.localScale = Vector3.one * spRadius;
		}
	}
	

}
