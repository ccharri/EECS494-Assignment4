using UnityEngine;
using System.Collections;

public class Projectile : Unit 
{
    Attribute speed;
    Creep target;
    Vector3 targetPos;

	public float getSpeed()
	{
		return (float)speed.get();
	}

	public override void Update() 
    {
        base.Update();
	}
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
