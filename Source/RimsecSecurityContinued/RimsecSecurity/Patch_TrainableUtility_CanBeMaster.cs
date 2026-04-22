using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(TrainableUtility), nameof(TrainableUtility.CanBeMaster))]
    public static class Patch_TrainableUtility_CanBeMaster
    {
        static bool Prefix(Pawn master, Pawn animal, ref bool __result)
        {
            if (master != null && PeacekeeperUtility.IsPeacekeeper(master))
            {
                __result = false;
                return false; // skip vanilla
            }

            return true; // run vanilla
        }
    }
}
