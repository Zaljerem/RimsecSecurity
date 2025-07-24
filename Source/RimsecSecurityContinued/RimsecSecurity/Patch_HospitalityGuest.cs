//using Hospitality.Utilities;
using RimWorld;
using Verse;

namespace RimsecSecurity;

public static class HospitalityPatches
{
    public static void ValidGuest_Postfix(Pawn pawn, Faction faction, ref bool __result)
    {
        if (!__result)
        {
            return;
        }

        // Check if the pawn is a Peacekeeper
        if (pawn.def.HasModExtension<RSPeacekeeperModExt>())
        {
            __result = false;
        }
    }
}
