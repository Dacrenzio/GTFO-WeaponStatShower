﻿namespace WeaponStatShower.Patches
{
    [Flags]
    public enum PatchType : byte
    {
        Prefix = 1,
        Postfix = 2,
        Both = Prefix | Postfix,
    }
}
