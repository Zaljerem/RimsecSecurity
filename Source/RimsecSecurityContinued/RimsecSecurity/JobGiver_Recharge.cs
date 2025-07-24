using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class JobGiver_Recharge : ThinkNode_JobGiver
{
	protected float minEnergyRechargePercentage = 100f;

	protected float minEnergyPowercellPercentage = 40f;

	protected override Job TryGiveJob(Pawn pawn)
	{
		if (pawn.Drafted)
		{
			return null;
		}
		if (pawn.needs.rest.CurLevel > minEnergyRechargePercentage / 100f)
		{
			return null;
		}
		Thing emptyChargeStation = PeacekeeperUtility.GetEmptyChargeStation(pawn);
		if (emptyChargeStation == null || emptyChargeStation.IsForbidden(pawn))
		{
			if (pawn.needs.rest.CurLevel < minEnergyPowercellPercentage / 100f)
			{
				Thing thing = PeacekeeperUtility.FindBestFuel(pawn);
				if (thing != null)
				{
					return JobMaker.MakeJob(RSDefOf.RSFuelRobot, pawn, thing);
				}
			}
			return JobMaker.MakeJob(JobDefOf.Wait, 120, checkOverrideOnExpiry: true);
		}
		return JobMaker.MakeJob(RSDefOf.RSRecharge, emptyChargeStation);
	}
}
