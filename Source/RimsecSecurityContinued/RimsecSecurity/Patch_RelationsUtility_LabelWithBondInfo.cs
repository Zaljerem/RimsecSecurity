using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(RelationsUtility), nameof(RelationsUtility.LabelWithBondInfo))]
    public static class Patch_RelationsUtility_LabelWithBondInfo
    {
        //this method tries to immediately check for relations, which Peacekeepers don't have
        static bool Prefix(Pawn humanlike, Pawn animal, ref string __result)
        {
            if (humanlike != null && PeacekeeperUtility.IsPeacekeeper(humanlike))
            {
                __result = humanlike.LabelShort;
                return false; // skip vanilla
            }

            return true; // run vanilla
        }
    }
}
