using UnityEngine;
using System.Collections;

public class Slowed : Buff
{
    private static float SLOW_FACTOR;

    public override void Awake()
    {
        base.Awake();
        Init(1);
        name = "Slowed";
    }

    public override void Init(int level_)
    {
        SLOW_FACTOR = 1 - Mathf.Pow(0.50f, level_);
        description = "+" + SLOW_FACTOR + " % slow";
        base.Init(level_);
    }

    public override void onApplication()
    {
        base.onApplication();
        Creep c = GetComponent<Creep>();
        if(c != null)
        {
            c.speed /= SLOW_FACTOR;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Creep c = GetComponent<Creep>();
        if(c != null)
        {
            c.speed *= SLOW_FACTOR;
        }
    }
}
