using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class CompFuelRobot : ThingComp
{
	public Pawn Parent => parent as Pawn;

	public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
	{
		if (Parent != null && !Parent.Dead)
		{
			AcceptanceReport acceptanceReport = CanRefuel(selPawn, null);
			yield return new FloatMenuOption("RSFuelRobotFloatMenu".Translate(), delegate
			{
				Job job = PeacekeeperUtility.RefuelJob(selPawn, Parent);
				job.count = 1;
				selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
			})
			{
				Disabled = !acceptanceReport.Accepted,
				revalidateClickTarget = Parent
			};
		}
	}

	private AcceptanceReport CanRefuel(Pawn pawn, object p)
	{
		if (!Parent.Map.itemAvailability.ThingsAvailableAnywhere(RSDefOf.RSPowerCell, 1, pawn))
		{
			return new AcceptanceReport("No fuel available");
		}
		return AcceptanceReport.WasAccepted;
	}
}
