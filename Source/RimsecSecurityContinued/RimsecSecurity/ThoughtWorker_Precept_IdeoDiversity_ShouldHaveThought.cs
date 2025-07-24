using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(ThoughtWorker_Precept_IdeoDiversity), "ShouldHaveThought")]
public static class Patch_ThoughtWorker_Precept_IdeoDiversity
{
    public static bool Prefix(Pawn p, ref ThoughtState __result)
    {
        if (ModSettings.removeIdeologyImpact && PeacekeeperUtility.IsPeacekeeper(p))
        {
            __result = ThoughtState.Inactive;
            return false; // skip original method
        }

        return true; // run original method
    }
}

