using HarmonyLib;
using RimWorld;

namespace RimsecSecurity
{ 

    
        [HarmonyPatch(typeof(InspirationHandler), "TryStartInspiration")]
        public static class Patch_TryStartInspiration
        {
            static bool Prefix(InspirationHandler __instance, ref bool __result)
            {
                if (PeacekeeperUtility.IsPeacekeeper(__instance.pawn))
                {
                    __result = false;
                    return false;  // Skip the original method
                }

                return true;  // Continue with the original method
            }
        }
 

}
