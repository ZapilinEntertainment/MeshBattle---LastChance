using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Destructible : MonoBehaviour {

	 protected float hp;
	public float maxHp;
	protected float armor;
	public float maxArmor;
	public float armorCoefficient;
	protected bool destroyed;

	public BoxCollider[] colliders;
	[SerializeField]
	protected float size = 1; //радиус сферы, в которую впишется объект
	public GameObject myRenderers;
	protected bool pooling;
	protected bool useTimer = false;
	protected float timer = 0;

	public Destructible () 
	{
		hp = maxHp;
		armor = maxArmor;
		destroyed = false;
		pooling = false;
	}
	public Destructible(float maxHP, int maxARMOR, float armorK, float mass, bool usePooling) 
	{
		maxHp = maxHP;
		hp = maxHp;
		maxArmor = maxARMOR;
		armor = maxArmor;
		armorCoefficient = armorK;
		pooling = usePooling;
	}

	void Awake () 
	{
		colliders = gameObject.GetComponents<BoxCollider>();
		hp = maxHp;
	}

	void Update () 
	{
		if (GameMaster.pause) return;
		if (Vector3.Distance (transform.position, Vector3.zero) > GameMaster.mapRadius) Destruction();
		else {
			if (useTimer) 
			{
				timer -= Time.deltaTime;
				if (timer<=0) {
					timer = 0;
					useTimer = false;
					Destruction();
				}
			}
		}
	}

	virtual public void Recreate() 
	{
		hp = maxHp;
		armor = maxArmor;
		destroyed = false;
		if (myRenderers != null) myRenderers.SetActive (true);
		else 
		{
			MeshRenderer mr = GetComponent<MeshRenderer>();
			if (mr != null) mr.enabled = true;
		}
		foreach (Collider c in colliders) c.enabled = true;
		gameObject.SetActive(true);
	}
		
	virtual public void Destruction () // doubled in Unit
	{
		if (destroyed) return; else destroyed=true;
		if (myRenderers) myRenderers.SetActive(false);
		else {
			MeshRenderer mr = GetComponent<MeshRenderer>();
			if (mr != null) mr.enabled = false;
		}

		foreach (BoxCollider c in colliders) {
		if (c.size.magnitude > 10) GameMaster.pool.DestructionAt (c, Vector3.zero);
		else GameMaster.pool.PiecesAt(transform.position, (int)c.size.magnitude);
		c.enabled = false;
		}

		if (!pooling) Destroy(gameObject); else 
		{
			transform.position = Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	public void ApplyDamage (Damage dmg)
	{
		if (destroyed) return;
		if (armor>0) 
		{
			armor -= dmg.penetration;
			hp -= dmg.damage*(1-armorCoefficient);
		}
		else
		{
			hp -= dmg.damage;
		}
		if (hp<0)
		{
			Destruction();
		}
	}

	public void SetPooling (bool x) {pooling = x;}
		
	public void SetData (float Hp, int Armor, float armorK, float mass, bool usePooling)
	{
		maxHp = Hp;
		hp = maxHp;
		maxArmor = Armor;
		armor = maxArmor;
		armorCoefficient = armorK;
		pooling = usePooling;
		Recreate();
	}

	public void UseTimer (float t) 
	{
		if (t == 0) {useTimer = false; timer = 0;}
		else {useTimer = true; timer = t;}
	}

	public float GetHpPercentage () {return (hp/ maxHp) * 100;}
	public float GetArmorPercentage () {return (armor/maxArmor) * 100;}
	public float GetSize() {return size;}
}
