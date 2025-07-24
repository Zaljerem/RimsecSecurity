using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(Alert_NeedColonistBeds), "GetReport")]
    public static class Patch_Alert_NeedColonistBeds
    {
        static void Postfix(ref AlertReport __result)
        {
            // Only proceed if the alert is active
            if (__result.active)
            {
                // Flag to determine if any valid colonist needs a bed
                bool anyColonistNeedingBed = false;

                // Iterate over all pawns in the player's faction on all maps
                foreach (Pawn pawn in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer))
                {
                    // Exclude robots, shamblers, prisoners, and slaves
                    if (!PeacekeeperUtility.IsPeacekeeper(pawn) &&
                        !pawn.IsShambler &&
                        !pawn.IsPrisoner &&
                        !pawn.IsSlave)
                    {
                        // Check if the pawn is a baby and needs a crib
                        if (pawn.DevelopmentalStage.Baby() &&
                            (pawn.ownership.OwnedBed == null || !pawn.ownership.OwnedBed.ForColonists))
                        {
                            anyColonistNeedingBed = true;
                            break;
                        }

                        // Check if the pawn lacks a colonist bed
                        if (pawn.ownership.OwnedBed == null || !pawn.ownership.OwnedBed.ForColonists)
                        {
                            anyColonistNeedingBed = true;
                            break;
                        }
                    }
                }

                // Deactivate the alert if no colonists need beds
                if (!anyColonistNeedingBed)
                {
                    __result = AlertReport.Inactive;
                }
            }
        }
    }
}
