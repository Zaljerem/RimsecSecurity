using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(GatheringsUtility), "ShouldGuestKeepAttendingGathering")]
public class GatheringsUtility_ShouldGuestKeepAttendingGathering
{
	public static bool Prefix(ref bool __result, Pawn p)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(p))
		{
			return true;
		}
		__result = false;
		return false;
	}
}
