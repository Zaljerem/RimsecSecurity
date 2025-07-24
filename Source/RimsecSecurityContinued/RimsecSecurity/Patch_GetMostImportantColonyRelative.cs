using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(PawnRelationUtility), "GetMostImportantColonyRelative")]
    public static class Patch_GetMostImportantColonyRelative
    {
        public static bool Prefix(Pawn pawn, ref Pawn __result)
        {
            // Null or invalid pawn check
            if (pawn == null || pawn.health?.hediffSet == null)
            {
                __result = null;
                return false; // Skip the original method
            }

            // Exclude robots from this method
            if (pawn.health.hediffSet.HasHediff(RSDefOf.RSRobotConsciousness))
            {
                __result = null;
                return false; // Skip the original method
            }

            try
            {
                // Safely get colony pawns and filter
                List<Pawn> colonyPawns = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists
                    .Where(x => x != null &&
                                x.RaceProps != null &&
                                x.RaceProps.Humanlike &&
                                x.relations != null &&
                                x.relations.DirectRelations != null &&
                                x.relations.DirectRelations.Any(r => r != null && r.otherPawn != null)) // Additional relation check
                    .ToList();

                // If no valid pawns are found, skip processing
                if (colonyPawns.Count == 0)
                {
                    __result = null;
                    return false; // Skip the original method
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"Exception in Patch_GetMostImportantColonyRelative: {ex.Message}\n{ex.StackTrace}");
                __result = null;
                return false; // Skip on error to prevent breaking the game
            }

            // Allow the original method to handle valid pawns
            return true;
        }
    }
}
