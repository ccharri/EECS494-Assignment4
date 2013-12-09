using UnityEngine;
using System.Collections;

public class Damage : Buff
{
    private static float DAMAGE_BOOST;

    public override void Awake()
    {
        base.Awake();
        Init(1);
    }

    public override void Init(int level_)
    {
        base.Init(level_);
        duration = 10;
        DAMAGE_BOOST = 10 * level_;
    }

    public override void onApplication()
    {
        Projectile p = GetComponent<Projectile>();
    }
    public override void onRemoval() { }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}