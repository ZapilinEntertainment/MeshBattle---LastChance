using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour {

	GUISkin defSkin, nbSkin;
	Material[] materials;
	Sprite[] spritePrefs;

	void Awake () 
	{
		GameMaster.storage = this;
		GameMaster.weaponsTypeCount = System.Enum.GetNames(typeof(WeaponType)).Length;

		//                  MATERIALS loading
		int count = System.Enum.GetNames(typeof (MaterialPurpose)).Length;
		materials = new Material[count];
		for (int i = 0; i < count; i++) {
			string name = System.Enum.GetName(typeof(MaterialPurpose), i);
			Material m = Resources.Load<Material>("Materials/" + name);
			if (m == null) print ("storage error - material " + name + " not found!");
			else 	materials[i] = m;
		}

		//        SPRITES loading
		count = System.Enum.GetNames(typeof (SpritePurpose)).Length;
		spritePrefs = new Sprite[count];
		for (int i = 0; i < count; i++) {
			string name = System.Enum.GetName(typeof(SpritePurpose), i);
			Sprite s = Resources.Load<Sprite>("Sprites/" + name);
			if (s == null) print ("storage error - sprite " + name + " not found!");
			else 	spritePrefs[i] = s;
		}
			
		defSkin = Resources.Load<GUISkin>("GUI/defSkin");
		nbSkin = Resources.Load<GUISkin>("GUI/noBorderSkin");
	}
		


	public Material GetMaterial(MaterialPurpose purpose) {
		Material m = materials[(int)purpose];
		if (m == null) m= materials[(int)MaterialPurpose.error_material];
		return m;
	}

	public Sprite GetSprite(SpritePurpose purpose) {
		Sprite s = spritePrefs[(int)purpose];
		if (s == null) s= spritePrefs[(int)SpritePurpose.error_sprite];
		return s;
	}

	public GUISkin GetDefaultSkin() {return defSkin;}
	public GUISkin GetNBSkin() {return nbSkin;}
		
}

public enum MaterialPurpose
{
	error_material,
	laser_ray_white,
	laser_ray_green
}

public enum SpritePurpose
{
	error_sprite,
	laser_splash_white,
	laser_splash_green
}
