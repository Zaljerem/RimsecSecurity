using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

[HarmonyPatch(typeof(WorkGiver_RescueDowned), "JobOnThing")]
public class WorkGiver_RescueDowned_JobOnThing
{
	public static void Postfix(ref Job __result, Pawn pawn, Thing t, bool forced = false)
	{
		Pawn pawn2 = t as Pawn;
		if (PeacekeeperUtility.IsPeacekeeper(pawn2) && !PeacekeeperUtility.IsInChargeStation(pawn2) && pawn.CanReserve(t))
		{
			Thing emptyChargeStation = PeacekeeperUtility.GetEmptyChargeStation(pawn2);
			if (emptyChargeStation != null)
			{
				Job job = JobMaker.MakeJob(RSDefOf.RSRescueToChargeStation, pawn2, emptyChargeStation);
				job.count = 1;
				__result = job;
			}
		}
	}
}
