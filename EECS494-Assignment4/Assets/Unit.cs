using UnityEngine;
using System.Collections;

public abstract class Unit : MonoBehaviour 
{	
	public override void Update () 
	{
		base.Update();
		//other things yay!
	}
	public override void FixedUpdate() 
	{
		base.FixedUpdate();
	}
}
