using HarmonyLib;
using Verse;
using RimWorld;
using System.Reflection;

namespace RimsecSecurity
{
    public static class Patch_TryAppendRelationsWithColonistsInfo
    {
        public static void ApplyPatch(Harmony harmony)
        {
            // Get the method to patch
            MethodInfo targetMethod = typeof(PawnRelationUtility).GetMethod(
                nameof(PawnRelationUtility.TryAppendRelationsWithColonistsInfo),
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(TaggedString).MakeByRefType(), typeof(Pawn) },
                null
            );

            // Get the prefix method
            MethodInfo prefixMethod = typeof(Patch_TryAppendRelationsWithColonistsInfo).GetMethod(
                nameof(Prefix),
                BindingFlags.Public | BindingFlags.Static
            );

            // Apply the patch
            harmony.Patch(targetMethod, new HarmonyMethod(prefixMethod));
        }

        public static bool Prefix(ref TaggedString text, Pawn pawn, ref bool __result)
        {
            //Log.Message("Patch_TryAppendRelationsWithColonistsInfo executed");

            // If the pawn has the RSRobotConsciousness hediff, skip
            //if (pawn.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true)
            //{

            if (PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists.Any(pawn => PeacekeeperUtility.IsPeacekeeper(pawn)))
            {
                __result = false;
                return false; // Skip original method
            }

            // Allow the original method to run
            return true;
        }
    }
}
