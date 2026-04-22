using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(PawnRelationUtility), nameof(PawnRelationUtility.Notify_PawnsSeenByPlayer))]
    public static class Patch_Notify_PawnsSeenByPlayer
    {
        public static void Prefix(ref IEnumerable<Pawn> seenPawns)
        {
            if (seenPawns == null)
                return;

            // MATERIALIZE to avoid deferred execution issues
            seenPawns = seenPawns
                .Where(p =>
                    p != null &&
                    p.relations != null &&
                    !PeacekeeperUtility.IsPeacekeeper(p)
                )
                .ToList();
        }
              



    }
}
