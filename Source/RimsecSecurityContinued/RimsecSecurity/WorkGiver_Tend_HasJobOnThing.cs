using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(WorkGiver_Tend), "HasJobOnThing")]
public class WorkGiver_Tend_HasJobOnThing
{
	public static void Postfix(ref bool __result, Pawn pawn, Thing t, bool forced = false)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(t as Pawn))
		{
			__result = false;
		}
	}
}
