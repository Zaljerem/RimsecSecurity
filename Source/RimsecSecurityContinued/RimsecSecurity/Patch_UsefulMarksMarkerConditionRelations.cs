using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace RimsecSecurity
{
    public static class Patch_UsefulMarksMarkerConditionRelations
    {
        public static void Apply(Harmony harmony)
        {
            var type = AccessTools.TypeByName("UsefulMarks.MarkerConditionRelations");
            if (type == null) return; // Mod not loaded — skip entirely

            var method = AccessTools.Method(type, "InternalSolveFor");
            if (method == null) return; // Method not found — skip

            harmony.Patch(
                method,
                prefix: new HarmonyMethod(typeof(Patch_UsefulMarksMarkerConditionRelations), nameof(Prefix))
            );
            Log.Message("[RimsecSecurity] Patching Useful Marks");
        }

        public static bool Prefix(Pawn pawn, ref bool __result)
        {
            if (pawn == null || pawn.IsColonist || pawn.relations == null)
            {
                __result = false;
                return false; // Skip original
            }

            var allPawns = PawnsFinder
                .AllMapsCaravansAndTravellingTransporters_Alive_FreeColonistsAndPrisoners
                .Where(x => x?.relations != null && x.relations.everSeenByPlayer)
                .ToList();

            foreach (var item in allPawns)
            {
                if (pawn == item)
                {
                    __result = false;
                    return false;
                }
                if (item.GetMostImportantRelation(pawn) != null)
                {
                    __result = true;
                    return false;
                }
            }

            __result = false;
            return false;
        }
    }
}
