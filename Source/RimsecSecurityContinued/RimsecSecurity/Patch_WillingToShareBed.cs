using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(BedUtility), "WillingToShareBed")]
public class Patch_WillingToShareBed
{
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result, Pawn pawn1, Pawn pawn2)
    {
        __result = PeacekeeperUtility.IsPeacekeeper(pawn1) || PeacekeeperUtility.IsPeacekeeper(pawn2);
        return !__result;
    }
}

