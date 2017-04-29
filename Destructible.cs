using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Destructible : MonoBehaviour {

	float hp;
	float maxHp;
	int armor;
	int maxArmor;
	float armorCoefficient;
	bool destroyed;

	Vector3 physicsMoveVector;
	Vector3 physicsRotateVector;
	protected float physicsMass;

	public BoxCollider mainCollider;
	public GameObject myRenderers;
	bool pooling;

	public float setHp;

	public Destructible () 
	{
		hp = maxHp;
		armor = maxArmor;
		destroyed = false;
		physicsMass = 1;
		pooling = false;
	}
	public Destructible(float maxHP, int maxARMOR, float armorK, float mass, bool usePooling) 
	{
		maxHp = maxHP;
		hp = maxHp;
		maxArmor = maxARMOR;
		armor = maxArmor;
		armorCoefficient = armorK;
		physicsMass = mass;
		pooling = usePooling;
	}

	void Awake () 
	{
		if (mainCollider == null) mainCollider = GetComponent<BoxCollider>();
		physicsMass = mainCollider.size.magnitude;
		maxHp = setHp;
		hp = maxHp;
	}

	void Update () 
	{
		if (GameMaster.pause) return;

		PhysicCalculation (Time.deltaTime);
	}

	protected void PhysicCalculation( float t) {
		if (physicsMoveVector != Vector3.zero) 
		{
			transform.Translate(physicsMoveVector * t, Space.World);
			float mgn = physicsMoveVector.magnitude;
			if (mgn < physicsMass) physicsMoveVector = Vector3.zero;
			else {
				mgn -= physicsMass * t;
				physicsMoveVector = physicsMoveVector.normalized * mgn;
			}
		}
		if (physicsRotateVector != Vector3.zero)
		{
			transform.Rotate (physicsRotateVector * t, Space.Self);
			float rmg = physicsRotateVector.magnitude;
			if (rmg < physicsMass) physicsRotateVector = Vector3.zero;
			else {
				rmg -= physicsMass * t;
				physicsRotateVector = physicsRotateVector.normalized * rmg;
			}
		}
	}


	public void ApplyPhysics (Vector3 hitVector, Vector3 hitPos) 
	{
		float damage_coefficient = 1 - (hitVector.normalized + Vector3.forward).magnitude/2.0f;
		float damage = (hitVector.magnitude - physicsMass) * damage_coefficient;
		print(damage);
		ApplyDamage(new Damage(damage, armor*10, hitPos));
		physicsMoveVector += 2*hitVector;
		physicsRotateVector += 2* (hitPos - transform.position+Random.onUnitSphere);
	}
		

	public void Recreate() 
	{
		hp = maxHp;
		armor = maxArmor;
		destroyed = false;
		physicsMoveVector = physicsRotateVector = Vector3.zero;
		myRenderers.SetActive (true);
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

	public void Destruction () 
	{
		if (destroyed) return; else destroyed=true;
		if (myRenderers) myRenderers.SetActive(false);
		if (mainCollider != null) GameMaster.pool.DestructionAt (mainCollider);
		if (!pooling) Destroy(gameObject); else gameObject.SetActive(false);
	}

	public void SetPooling (bool x) {pooling = x;}
		
	public void SetData (float Hp, int Armor, float armorK, float mass, bool usePooling)
	{
		maxHp = Hp;
		hp = maxHp;
		maxArmor = Armor;
		armor = maxArmor;
		armorCoefficient = armorK;
		physicsMass = mass;
		pooling = usePooling;
	}

	public float GetMass () {return physicsMass;}
}
