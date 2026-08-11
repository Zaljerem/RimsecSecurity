using Verse;

namespace RimsecSecurity;

public static class RimTalkPatches
{
    public static bool IsTalkEligible_Prefix(ref bool __result, Pawn pawn)
    {
        // Skip for Peacekeepers
        if (PeacekeeperUtility.IsPeacekeeper(pawn))
        {
            __result = false;  // Directly override result
            return false;  // Skip original method
        }      

        return true;
    }
}
