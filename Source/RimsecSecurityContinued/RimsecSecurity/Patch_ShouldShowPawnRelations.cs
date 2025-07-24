using HarmonyLib;
using Verse;
using RimWorld;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(SocialCardUtility), "ShouldShowPawnRelations")]
    public static class Patch_ShouldShowPawnRelations
    {
        public static bool Prefix(Pawn pawn, Pawn selPawnForSocialInfo, ref bool __result)
        {

            //Log.Message("Patch_ShouldShowPawnRelations");

            // If the pawn has the RSRobotConsciousness hediff, skip the relations check
            //if (pawn.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true ||
                //selPawnForSocialInfo.health?.hediffSet?.HasHediff(RSDefOf.RSRobotConsciousness) == true)
                if (PeacekeeperUtility.IsPeacekeeper(pawn) ||
                    PeacekeeperUtility.IsPeacekeeper(selPawnForSocialInfo))
                {
                __result = false;
                return false;
            }

            // Allow the original method to run
            return true;
        }
    }
}

