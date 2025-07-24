using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(RitualBehaviorWorker_ChildBirth), "ShouldInitAsSpectator")]
    public static class Patch_RitualBehaviorWorker_ChildBirth_ShouldInitAsSpectator
    {
        static bool Prefix(ref bool __result, Pawn pawn)
        {
            // Check if the pawn is a robot
            if (PeacekeeperUtility.IsPeacekeeper(pawn))
            {
                __result = false; // Prevent robots from being spectators in childbirth
                return false; // Skip the original method
            }

            // Allow the original method to execute for non-robot pawns
            return true;
        }
    }
}
