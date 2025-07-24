using System;
using RimWorld;
using Verse;

namespace RimsecSecurity;

internal static class DubsHygienePatches
{
	public static void Pawn_NeedsTracker_ShouldHaveNeed_Postfix(Pawn_NeedsTracker __instance, ref bool __result, Pawn ___pawn, NeedDef nd)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(___pawn) && (bool)PatchesCompatibility.hygieneAssembly.GetType("DubsBadHygiene.NeedsUtil").GetMethod("IsHygieneNeed", new Type[1] { typeof(NeedDef) }).Invoke(null, new object[1] { nd }))
		{
			__result = false;
		}
	}
}
