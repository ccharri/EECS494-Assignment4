﻿using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SplashBuster : Amplifer
{
    protected override Buff addBuff(Tower t)
    {
        Buff b = t.gameObject.AddComponent<DamageBoost>();
        b.Init(buffAppliedLevel);
        b.onApplication();
        return b;
    }
}
