using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(ThoughtWorker_LookChangeDesired), "CurrentStateInternal")]
    public static class Patch_ThoughtWorker_LookChangeDesired
    {
        static bool Prefix(Pawn p, ref ThoughtState __result)
        {
            if (PeacekeeperUtility.IsPeacekeeper(p))
            {
                __result = ThoughtState.Inactive;
                return false; // Skip the original method execution
            }
            return true; // Continue with original method if not a Peacekeeper
        }
    }
}
