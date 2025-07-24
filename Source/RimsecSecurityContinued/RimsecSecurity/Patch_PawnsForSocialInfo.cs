using HarmonyLib;
using RimsecSecurity;
using RimWorld;
using System.Collections.Generic;
using System;
using Verse;

[HarmonyPatch(typeof(SocialCardUtility), "PawnsForSocialInfo")]
public static class Patch_PawnsForSocialInfo
{
    public static bool Prefix(Pawn pawn, ref IEnumerable<Pawn> __result)
    {
        try
        {
            if (pawn?.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true)
            {
                //Log.Message($"[RimsecSecurity] Skipping PawnsForSocialInfo for robot: {pawn.LabelShort}");
                __result = new List<Pawn>();
                return false; // Skip original method
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[RimsecSecurity] Exception in PawnsForSocialInfo prefix: {ex}");
        }

        return true; // Run original method
    }
}
