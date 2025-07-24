using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(Pawn_IdeoTracker), "SetIdeo")]
    public static class Patch_Pawn_IdeoTracker_SetIdeo
    {
        static bool Prefix(Pawn_IdeoTracker __instance)
        {
            // Access the private 'pawn' field from the instance
            var pawnField = AccessTools.Field(typeof(Pawn_IdeoTracker), "pawn");
            var pawn = (Pawn)pawnField.GetValue(__instance);

            // Check if the pawn is a Peacekeeper
            if (PeacekeeperUtility.IsPeacekeeper(pawn))
            {
                // Prevent the original method from running
                return false;
            }

            // Allow the original method to run
            return true;
        }
    }
}
