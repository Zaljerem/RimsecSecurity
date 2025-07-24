using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

[HarmonyPatch(typeof(WorkGiver_RescueDowned), "HasJobOnThing")]
public class WorkGiver_RescueDowned_HasJobOnThing
{
	public static void Postfix(ref bool __result, Pawn pawn, Thing t, bool forced = false)
	{
		Pawn pawn2 = t as Pawn;
		if (PeacekeeperUtility.IsPeacekeeper(pawn2))
		{
			if (!pawn2.Downed || pawn2.Faction != Faction.OfPlayer || PeacekeeperUtility.IsInChargeStation(pawn2) || !pawn.CanReserve(pawn2) || GenAI.EnemyIsNear(pawn2, 40f))
			{
				__result = false;
				return;
			}
			Thing emptyChargeStation = PeacekeeperUtility.GetEmptyChargeStation(pawn2);
			__result = emptyChargeStation != null && pawn2.CanReserve(emptyChargeStation);
		}
	}
}
