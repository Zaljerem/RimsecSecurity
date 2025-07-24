using Verse;

namespace RimsecSecurity;

public static class OrphanagePatches
{
    public static bool PawnCounts_Prefix(ref bool __result, Pawn pawn)
    {
        // Skip for Peacekeepers
        if (PeacekeeperUtility.IsPeacekeeper(pawn))
        {
            __result = false;  // Directly override result
            return false;  // Skip original method
        }

        // Handle null relations safely
        if (pawn.relations == null)
        {
            __result = false;
            return false;
        }

        return true;
    }
}
