using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

[HarmonyPatch(typeof(JobGiver_RescueNearby), "TryGiveJob")]
public class JobGiver_RescueNearby_TryGiveJob
{
	public static void Postfix(ref Job __result, Pawn pawn, float ___radius)
	{
		if (__result != null)
		{
			return;
		}
		Predicate<Thing> validator = delegate(Thing t)
		{
			Pawn pawn3 = (Pawn)t;
			return pawn3.Downed && pawn3.Faction == pawn.Faction && !pawn3.InBed() && pawn.CanReserve(pawn3) && !pawn3.IsForbidden(pawn) && !GenAI.EnemyIsNear(pawn3, 25f);
		};
		Pawn pawn2 = (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(pawn), ___radius, validator);
		if (pawn2 != null && PeacekeeperUtility.IsPeacekeeper(pawn2) && !PeacekeeperUtility.IsInChargeStation(pawn2))
		{
			Thing emptyChargeStation = PeacekeeperUtility.GetEmptyChargeStation(pawn2);
			if (emptyChargeStation != null && pawn2.CanReserve(emptyChargeStation) && !emptyChargeStation.IsForbidden(pawn))
			{
				Job job = JobMaker.MakeJob(RSDefOf.RSRescueToChargeStation, pawn2, emptyChargeStation);
				job.count = 1;
				__result = job;
			}
		}
	}
}
