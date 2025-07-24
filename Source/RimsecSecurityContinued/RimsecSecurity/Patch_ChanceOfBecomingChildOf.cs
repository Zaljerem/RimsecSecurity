using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(ChildRelationUtility))]
    [HarmonyPatch(nameof(ChildRelationUtility.ChanceOfBecomingChildOf))]
    public static class Patch_ChanceOfBecomingChildOf
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Pawn child, Pawn father, Pawn mother)
        {
            // Check if any of the involved pawns are robots
            if ((child != null && PeacekeeperUtility.IsPeacekeeper(child)) ||
                (father != null && PeacekeeperUtility.IsPeacekeeper(father)) ||
                (mother != null && PeacekeeperUtility.IsPeacekeeper(mother)))
            {
                __result = 0f; // Return 0 chance
                return false;  // Skip original method
            }
            return true;      // Proceed with the original method
        }
    }
}
