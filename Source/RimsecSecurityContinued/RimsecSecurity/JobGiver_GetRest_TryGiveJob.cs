using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

[HarmonyPatch(typeof(JobGiver_GetRest), "TryGiveJob")]
public class JobGiver_GetRest_TryGiveJob
{
	public static void Postfix(ref Job __result, Pawn pawn)
	{
		if (__result != null && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = null;
		}
	}
}
