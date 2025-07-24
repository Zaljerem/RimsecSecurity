using HarmonyLib;
using RimsecSecurity;
using RimWorld;
using Verse;

[HarmonyPatch(typeof(PawnUtility), "ShouldGetThoughtAbout")]
public static class Patch_ShouldGetThoughtAbout
{
    static bool Prefix(ref bool __result, Pawn pawn, Pawn subject)
    {
        // Check if either pawn or subject is a robot
        if (PeacekeeperUtility.IsPeacekeeper(pawn) || PeacekeeperUtility.IsPeacekeeper(subject))
        {
            __result = false; // Robots don't get or generate thoughts
            return false;     // Skip the original method
        }

        return true; // Continue with the original method for non-robots
    }
}

