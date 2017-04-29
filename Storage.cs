using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour {

	Material[] materials;
	SpriteRenderer[] spritePrefs;

	void Awake () 
	{
		GameMaster.storage = this;

		//                  MATERIALS loading
		int count = System.Enum.GetNames(typeof (MaterialPurpose)).Length;
		materials = new Material[count];
		for (int i = 0; i < count; i++) {
			string name = System.Enum.GetName(typeof(MaterialPurpose), i);
			Material m = Resources.Load<Material>("Materials/" + name);
			if (m == null) print ("storage error - material " + name + " not found!");
			else 	materials[i] = m;
		}
	}
		


	public Material GetMaterial(MaterialPurpose purpose) {
		Material m = materials[(int)purpose];
		if (m == null) m= materials[(int)MaterialPurpose.error_material];
		return m;
	}
}

public enum MaterialPurpose
{
	error_material,
	laser_ray_white,
	laser_ray_green
}

public enum SpritePurpose
{
	laser_splash_white
}
