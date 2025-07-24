using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Alert_ColonistNeedsRescuing), "NeedsRescue")]
public class Alert_ColonistNeedsRescuing_NeedsRescue
{
	public static void Postfix(ref bool __result, Pawn p)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(p) && PeacekeeperUtility.IsInChargeStation(p))
		{
			__result = false;
		}
	}
}
