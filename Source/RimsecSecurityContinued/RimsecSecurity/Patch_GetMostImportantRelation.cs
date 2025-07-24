using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch]
    public static class Patch_GetMostImportantRelation
    {
        // Target the extension method manually
        static System.Reflection.MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(PawnRelationUtility), "GetMostImportantRelation", new[] { typeof(Pawn), typeof(Pawn) });
        }

        static bool Prefix(ref PawnRelationDef __result, Pawn me, Pawn other)
        {
            // Null checks for safety
            if (me == null || other == null)
            {
                __result = null;
                return false; // Skip original if either pawn is null
            }

            // Check if either pawn is a peacekeeper/robot
            if (PeacekeeperUtility.IsPeacekeeper(me) || PeacekeeperUtility.IsPeacekeeper(other))
            {
                __result = null;
                return false;  // Skip the original method
            }

            return true; // Allow original method to execute
        }
    }
}
