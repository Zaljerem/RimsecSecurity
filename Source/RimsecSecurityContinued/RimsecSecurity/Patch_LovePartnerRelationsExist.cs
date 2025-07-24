using HarmonyLib;
using RimsecSecurity;
using RimWorld;
using Verse;

[HarmonyPatch(typeof(LovePartnerRelationUtility), "LovePartnerRelationExists")]
public static class Patch_LovePartnerRelationExists
{
    static bool Prefix(ref bool __result, Pawn first, Pawn second)
    {
        // Check if either pawn or subject is a robot
        if (PeacekeeperUtility.IsPeacekeeper(first) || PeacekeeperUtility.IsPeacekeeper(second))
        {
            __result = false; // Robots don't get or generate thoughts
            return false;     // Skip the original method
        }

        return true; // Continue with the original method for non-robots
    }
}
