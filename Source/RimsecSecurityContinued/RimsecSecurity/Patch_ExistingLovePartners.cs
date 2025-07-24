using HarmonyLib;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace RimsecSecurity;

[HarmonyPatch(typeof(LovePartnerRelationUtility), "ExistingLovePartners")] 
public static class Patch_ExistingLovePartners
{
    public static bool Prefix(Pawn pawn, bool allowDead, ref List<DirectPawnRelation> __result)
    {
        //Log.Message("Patch_ExistingLovePartners");

        // Check if the pawn has the RSRobotConsciousness hediff
        if (pawn.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true)
        {
            // Return an empty list and skip the original method
            __result = new List<DirectPawnRelation>();
            return false;
        }

        // Allow the original method to run
        return true;
    }
}
