using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(LovePartnerRelationUtility), "IncestOpinionOffsetFor")]
    public static class Patch_LovePartnerRelationUtility_IncestOpinionOffsetFor
    {
        static bool Prefix(Pawn other, Pawn pawn, ref float __result)
        {
            // Check if either pawn is a robot
            if (PeacekeeperUtility.IsPeacekeeper(other) || PeacekeeperUtility.IsPeacekeeper(pawn))
            {
                // If so, override the result to 0 and skip the original method
                __result = 0f;
                return false;
            }

            // Allow the original method to execute if neither pawn is a robot
            return true;
        }
    }
}

