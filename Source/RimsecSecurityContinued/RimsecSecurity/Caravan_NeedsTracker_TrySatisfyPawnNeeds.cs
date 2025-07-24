using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Caravan_NeedsTracker), "TrySatisfyPawnNeeds")]
public class Caravan_NeedsTracker_TrySatisfyPawnNeeds
{
	public static void Postfix(Pawn pawn, Caravan ___caravan)
	{
		if (!pawn.Dead && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			PeacekeeperUtility.RefuelPawnOnCaravan(pawn, ___caravan);
		}
	}
}
