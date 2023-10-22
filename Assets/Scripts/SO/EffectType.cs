using System;

namespace YuriGameJam2023.SO
{
    [Flags]
    public enum EffectType
    {
        None,
        Everything = ~0,

        Poison = 1 << 0,
        Spiderweb = 1 << 1,
        Fire = 1 << 2
    }
}
