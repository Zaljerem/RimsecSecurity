using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{

    // No observed corpse history/thought if a peacekeeper

    [HarmonyPatch(typeof(Corpse), nameof(Corpse.GiveObservedHistoryEvent))]
    public static class Patch_Corpse_GiveObservedHistoryEvent
    {
        static bool Prefix(Corpse __instance, ref HistoryEventDef __result)
        {
            if (PeacekeeperUtility.IsPeacekeeper(__instance.InnerPawn))
            {
                __result = null;
                return false;
            }

            return true;
        }
    }
}
