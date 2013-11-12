using UnityEngine;
using System.Collections;

public abstract class ProjectileBuff : Buff 
{
    protected Projectile target;

    public ProjectileBuff(Projectile target_) : base()
    {
        target = target_;
        onApplication();
    }
    public ProjectileBuff(Projectile target_, Unit owner_) : base(owner_)
    {
        target = target_;
        onApplication();
    }

    //NOTE: A ProjectileBuff is meaningless without a target!
    private ProjectileBuff() {}
}
