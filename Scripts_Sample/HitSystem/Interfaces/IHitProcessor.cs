using System.Collections;
using UnityEngine;
using SSW.Monster;

namespace SSW.HitSystem
{
    public interface IHitProcessor
    {
        bool CanBeHit(MonsterControllerRefactored controller);
        IEnumerator CoProcessHit(MonsterControllerRefactored controller, HitData hitData);
    }
}
