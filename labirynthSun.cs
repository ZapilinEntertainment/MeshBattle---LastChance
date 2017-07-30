using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class labirynthSun : MonoBehaviour {
	public float daytime = 0.5f;
	public float speed = 0.1f;
	public Light sun;
	public GameObject pseudoSun;
	// Use this for initialization
	void Start () {
			sun.transform.rotation = Quaternion.Euler(Vector3.right * 180 * daytime);
			pseudoSun.SetActive(false);
			pseudoSun.transform.rotation = Quaternion.Euler(Vector3.right * 180 * daytime);
	}
	
	// Update is called once per frame
	void Update () {
			daytime += speed * Time.deltaTime;
		if (daytime >= 2) {daytime -= 2;pseudoSun.SetActive(false);}
		else {
			if (daytime >= 1) {				
				pseudoSun.SetActive(true);
				sun.intensity = 0.3f;
			}
			else {
				sun.intensity = 1+3*(1 - 2* Mathf.Abs(daytime - 0.5f));
			}
		}
		sun.transform.rotation = Quaternion.Euler(Vector3.right * 180 * daytime);
		pseudoSun.transform.position = -sun.transform.forward* 100;
	}
}
