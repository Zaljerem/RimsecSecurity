using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class JobDriver_FuelRobot : JobDriver
{
	private const TargetIndex RefuelableInd = TargetIndex.A;

	private const TargetIndex FuelInd = TargetIndex.B;

	private const int RefuelingDuration = 240;

	private IntVec3 startingPos;

	protected Pawn Refuelable => job.GetTarget(TargetIndex.A).Pawn;

	protected Thing Fuel => job.GetTarget(TargetIndex.B).Thing;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		if (pawn.Reserve(Refuelable, job, 1, -1, null, errorOnFailed))
		{
			return pawn.Reserve(Fuel, job, 1, -1, null, errorOnFailed);
		}
		return false;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		startingPos = Refuelable.Position;
		this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
		AddEndCondition(() => ((double)Refuelable.needs.rest.CurLevel < 0.9 || (Refuelable == pawn && pawn.Position != startingPos && pawn.CanReach(startingPos, PathEndMode.OnCell, Danger.Deadly))) ? JobCondition.Ongoing : JobCondition.Succeeded);
		yield return Toils_General.DoAtomic(delegate
		{
			job.count = Convert.ToInt32((Refuelable.needs.rest.MaxLevel - Refuelable.needs.rest.CurLevel) * 10f);
		}).FailOn(() => job.count == 0);
		Toil reserveFuel = Toils_Reserve.Reserve(TargetIndex.B);
		yield return reserveFuel;
		yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
		yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
		yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveFuel, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
		yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
			.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
			.WithProgressBarToilDelay(TargetIndex.A);
		Toil toil = new Toil();
		toil.initAction = delegate
		{
			Refuelable.needs.rest.CurLevel += (float)Fuel.stackCount / 10f;
			Fuel.Destroy();
		};
		toil.defaultCompleteMode = ToilCompleteMode.Instant;
		yield return toil;
		yield return Toils_Goto.GotoCell(startingPos, PathEndMode.OnCell).FailOn(() => pawn != Refuelable);
	}
}
