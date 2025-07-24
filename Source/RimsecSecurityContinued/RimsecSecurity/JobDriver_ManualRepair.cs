using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class JobDriver_ManualRepair : JobDriver
{
	protected Building_ChargeStation Station => job.GetTarget(TargetIndex.A).Thing as Building_ChargeStation;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		return pawn.Reserve(job.targetB, job, 1, -1, null, errorOnFailed);
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOn(() => pawn.Drafted).FailOnDespawnedNullOrForbidden(TargetIndex.B);
		yield return Toils_General.Wait(Station.CurrentRobot.def.GetModExtension<RSPeacekeeperModExt>().repairTicks).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
			.WithProgressBarToilDelay(TargetIndex.A)
			.WithEffect(EffecterDefOf.ConstructMetal, TargetIndex.A);
		yield return new Toil
		{
			initAction = delegate
			{
				FullyRepair(Station.CurrentRobot);
				Station.CompFuel.ConsumeFuel(Station.CompRecharge.ComponentsForManualRepair);
			}
		}.FailOn(() => Station.CurrentRobot == null || Station.CompRecharge.ComponentsForManualRepair == 0f);
	}

	private void FullyRepair(Pawn currentRobo)
	{
		FleckMaker.ThrowDustPuffThick(currentRobo.Position.ToVector3(), currentRobo.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
		foreach (Hediff item in Enumerable.Reverse(currentRobo.health.hediffSet.hediffs))
		{
			if (item is Hediff_Injury || item is Hediff_MissingPart)
			{
				HealthUtility.Cure(item);
			}
		}
	}
}
