using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using RimsecSecurity;
using System;

[HarmonyPatch(typeof(SocialCardUtility), "Recache")]
public static class Patch_SocialCardUtility_Recache
{
    public static bool Prefix(Pawn selPawnForSocialInfo)
    {
        try
        {
            // Check if the pawn is a robot
            if (selPawnForSocialInfo?.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true)
            {
                // Use reflection to access private fields
                FieldInfo cachedForPawnField = typeof(SocialCardUtility).GetField("cachedForPawn", BindingFlags.Static | BindingFlags.NonPublic);
                FieldInfo tmpToCacheField = typeof(SocialCardUtility).GetField("tmpToCache", BindingFlags.Static | BindingFlags.NonPublic);
                FieldInfo cachedEntriesField = typeof(SocialCardUtility).GetField("cachedEntries", BindingFlags.Static | BindingFlags.NonPublic);
                FieldInfo cachedRolesField = typeof(SocialCardUtility).GetField("cachedRoles", BindingFlags.Static | BindingFlags.NonPublic);

                // Get field values
                cachedForPawnField.SetValue(null, selPawnForSocialInfo);
                List<Pawn> tmpToCache = (List<Pawn>)tmpToCacheField.GetValue(null);
                List<object> cachedEntries = (List<object>)cachedEntriesField.GetValue(null);
                List<Precept_Role> cachedRoles = (List<Precept_Role>)cachedRolesField.GetValue(null);

                // Clear cached data
                tmpToCache.Clear();
                cachedEntries.Clear();
                cachedRoles.Clear();

                // Log for debugging
                //Log.Message($"[RimsecSecurity] Recache skipped for robot pawn: {selPawnForSocialInfo.LabelShort}");
                return false; // Skip the original method
            }

            // Log for non-robot pawns
            //Log.Message($"[RimsecSecurity] Recache running for pawn: {selPawnForSocialInfo?.LabelShort ?? "NULL"}");
        }
        catch (Exception ex)
        {
            Log.Error($"[RimsecSecurity] Exception in Recache prefix: {ex}");
        }

        return true; // Run the original method for non-robots
    }

    private static void ClearCachedEntry(object cachedEntry)
    {
        // Use reflection to clear fields of CachedSocialTabEntry
        FieldInfo otherPawnField = cachedEntry.GetType().GetField("otherPawn", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo opinionOfOtherPawnField = cachedEntry.GetType().GetField("opinionOfOtherPawn", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo opinionOfMeField = cachedEntry.GetType().GetField("opinionOfMe", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo relationsField = cachedEntry.GetType().GetField("relations", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo pregnancyApproachField = cachedEntry.GetType().GetField("pregnancyApproach", BindingFlags.Instance | BindingFlags.Public);

        otherPawnField?.SetValue(cachedEntry, null);
        opinionOfOtherPawnField?.SetValue(cachedEntry, 0);
        opinionOfMeField?.SetValue(cachedEntry, 0);
        ((List<PawnRelationDef>)relationsField?.GetValue(cachedEntry))?.Clear();
        pregnancyApproachField?.SetValue(cachedEntry, null);
    }
}
