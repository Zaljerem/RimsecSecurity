using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(Alert_ColonistLeftUnburied), nameof(Alert_ColonistLeftUnburied.IsCorpseOfColonist))]
    public static class Patch_Alert_ColonistLeftUnburied_IsCorpseOfColonist
    {
        static bool Prefix(Corpse corpse, ref bool __result)
        {
            if (corpse?.InnerPawn != null && PeacekeeperUtility.IsPeacekeeper(corpse.InnerPawn))
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
}