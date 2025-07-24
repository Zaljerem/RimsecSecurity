using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(PawnBreathMoteMaker), "TryMakeBreathMote")]
    public static class Patch_PawnBreathMoteMaker_TryMakeBreathMote
    {
        static bool Prefix(PawnBreathMoteMaker __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            // Check if the pawn is a robot
            if (pawn != null && PeacekeeperUtility.IsPeacekeeper(pawn))
            {
                // Skip the original method if the pawn is a robot
                return false;
            }

            // Allow the original method to run for non-robot pawns
            return true;
        }
    }
}