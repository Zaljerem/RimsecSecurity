using HarmonyLib;
using Verse;

namespace RimsecSecurity
{
    // Prevent growth moments by simply never providing options and skipping the vanilla method.
    // Vanilla itself sets them to 0 before running logic to check for a growth moment.

    [HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.TryChildGrowthMoment))]
    public static class Patch_Pawn_AgeTracker_TryChildGrowthMoment
    {
        private static readonly AccessTools.FieldRef<Pawn_AgeTracker, Pawn> PawnRef =
            AccessTools.FieldRefAccess<Pawn_AgeTracker, Pawn>("pawn");

        static bool Prefix(
            Pawn_AgeTracker __instance,
            int birthdayAge,
            out int newPassionOptions,
            out int newTraitOptions,
            out int passionGainsCount)
        {
            Pawn pawn = PawnRef(__instance);

            if (PeacekeeperUtility.IsPeacekeeper(pawn))
            {
                newPassionOptions = 0;
                newTraitOptions = 0;
                passionGainsCount = 0;
                
                return false;
            }
            
            newPassionOptions = 0;
            newTraitOptions = 0;
            passionGainsCount = 0;
            return true;
        }
    }
}