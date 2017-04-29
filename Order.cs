using System;
using UnityEngine;

	public class Order
	{
	public OrderType type;
	public Destructible targetObj;
	public Unit targetUnit;
	public Vector3 targetPoint;

	public Order (OrderType otype, Destructible otarget)
		{
		type = otype;
		targetObj = otarget;
		}
	public Order (OrderType otype, Unit ounit,Vector3 opoint)
	{
		type = otype;
		targetUnit = ounit;
		targetPoint = opoint;
	}
	public Order (OrderType otype, Vector3 opoint)
	{
		type = otype;
		targetPoint = opoint;
	}
	public Order (OrderType otype) 
	{
		type = otype;
	}
	}

public enum OrderType 
{
	Stand,
	Move,
	Follow,
	FollowInFormation,
	Defend,
	Attack,
	Retreat
}


