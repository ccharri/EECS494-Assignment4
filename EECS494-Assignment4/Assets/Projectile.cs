using UnityEngine;
using System.Collections;

public abstract class Projectile : Unit 
{
    Attribute speed;
    Creep target;
    Vector3 targetPos;

    public Projectile()
    {
        tag = "Projectile";
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
