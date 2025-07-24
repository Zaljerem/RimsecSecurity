using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;
using RimWorld;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(PawnRelationUtility), "Notify_PawnsSeenByPlayer_Letter")]
    public static class Patch_Notify_PawnsSeenByPlayer_Letter
    {
        public static bool Prefix(ref IEnumerable<Pawn> seenPawns, ref TaggedString letterLabel, ref TaggedString letterText, string relationsInfoHeader, bool informEvenIfSeenBefore = false, bool writeSeenPawnsNames = true)
        {
            //Log.Message("Patch_Notify_PawnsSeenByPlayer_Letter");

            // Check if any colonists have the RSRobotConsciousness hediff
            //if (PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists.Any(pawn => pawn.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true))
                if (PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists.Any(pawn => PeacekeeperUtility.IsPeacekeeper(pawn)))
                {
                // Skip the original method
                //letterLabel = null;
                //letterText = null;
                return false;
            }

            // Allow the base method to run if no matching colonists are found
            return true;
        }
    }
}