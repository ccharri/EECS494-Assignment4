using UnityEngine;
using System.Collections;

public abstract class Sauce : MonoBehaviour
{
    public string name;
    public string id;
	public double duration;
    public Unit owner;

    public virtual void Awake()
    {
        
    }

    public virtual void OnDestroy()
    {

    }

    public virtual void FixedUpdate()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0)
            OnDestroy();
    }
}
