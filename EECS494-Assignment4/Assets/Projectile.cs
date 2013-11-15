using UnityEngine;
using System.Collections;

public abstract class Projectile : Unit 
{
    Attribute speed;
    Creep target;
    Vector3 targetPos;

<<<<<<< HEAD
	public float getSpeed()
	{
		return (float)speed.get();
	}
=======
    public Projectile()
    {
        tag = "Projectile";
    }
>>>>>>> 4b82ab7fcb6c7f937b14026cdeca08b4f25d09e1

	public override void Update() 
    {
        base.Update();
	}
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
