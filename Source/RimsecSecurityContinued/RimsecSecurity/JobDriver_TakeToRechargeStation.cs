using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class JobDriver_TakeToRechargeStation : JobDriver
{
	protected Pawn Takee => job.GetTarget(TargetIndex.A).Pawn;

	protected Thing DropBed => job.GetTarget(TargetIndex.B).Thing;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		if (pawn.Reserve(Takee, job, 1, -1, null, errorOnFailed))
		{
			return pawn.Reserve(DropBed, job, 1, 0, null, errorOnFailed);
		}
		return false;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		this.FailOnDestroyedOrNull(TargetIndex.A);
		this.FailOnDestroyedOrNull(TargetIndex.B);
		this.FailOnAggroMentalStateAndHostile(TargetIndex.A);
		this.FailOn(delegate
		{
			if (job.def.makeTargetPrisoner)
			{
				Log.Message("tried to make robot prisoner in " + GetType().Name);
				return true;
			}
			return false;
		});
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B)
			.FailOn(() => job.def == JobDefOf.Arrest && !Takee.CanBeArrestedBy(pawn))
			.FailOn(() => !pawn.CanReach(DropBed, PathEndMode.Touch, Danger.Deadly))
			.FailOn(() => job.def == RSDefOf.RSRescueToChargeStation && !Takee.Downed)
			.FailOnSomeonePhysicallyInteracting(TargetIndex.A);
		yield return Toils_Haul.StartCarryThing(TargetIndex.A);
		yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
		yield return Toils_Reserve.Release(TargetIndex.B);
		yield return new Toil
		{
			initAction = delegate
			{
				if (pawn.carryTracker.TryDropCarriedThing(((Building_ChargeStation)DropBed).GetStandPosition(Takee), ThingPlaceMode.Direct, out var _))
				{
					((Building_ChargeStation)DropBed).CurrentRobot = Takee;
				}
			},
			defaultCompleteMode = ToilCompleteMode.Instant
		};
	}
}
