using System.Collections;
using System.Collections.Generic;
using SSW.HitSystem;
using UnityEngine;
public interface IDamageable
{
    void OnHit(HitData hitData);
}