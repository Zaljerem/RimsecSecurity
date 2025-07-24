using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class JobDriver_Recharge : JobDriver
{
	private const int TargetSearchInterval = 4;

	protected Building_ChargeStation Station => job.GetTarget(TargetIndex.A).Thing as Building_ChargeStation;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		yield return Toils_Goto.GotoThing(TargetIndex.A, Station.GetStandPosition(pawn)).FailOn(() => pawn.Drafted).FailOnDespawnedNullOrForbidden(TargetIndex.A);
		Toil toil = new Toil();
		toil.defaultDuration = Rand.Range(180, 240);
		toil.defaultCompleteMode = ToilCompleteMode.Never;
		toil.handlingFacing = true;
		toil.initAction = delegate
		{
			base.Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
			pawn.pather.StopDead();
			Station.CurrentRobot = pawn;
		};
		toil.tickAction = delegate
		{
			if (ticksLeftThisToil <= 0 && pawn.needs.rest.CurLevel >= 0.99f)
			{
				ReadyForNextToil();
			}
			else if (job.expiryInterval == -1 && job.def == JobDefOf.Wait_Combat && !pawn.Drafted)
			{
				Log.Error(pawn?.ToString() + " in eternal WaitCombat without being drafted.");
				ReadyForNextToil();
			}
			else
			{
				if ((Find.TickManager.TicksGame + pawn.thingIDNumber) % 4 == 0)
				{
					CheckForAutoAttack();
				}
				IntVec3 position = pawn.Position;
				position.z--;
				pawn.rotationTracker.FaceCell(position);
			}
		};
		yield return toil;
	}

	public override void Notify_StanceChanged()
	{
		if (pawn.stances.curStance is Stance_Mobile)
		{
			CheckForAutoAttack();
		}
	}

	private void CheckForAutoAttack()
	{
		if (base.pawn.Downed || base.pawn.stances.FullBodyBusy)
		{
			return;
		}
		collideWithPawns = false;
		bool flag = !base.pawn.WorkTagIsDisabled(WorkTags.Violent);
		bool flag2 = base.pawn.RaceProps.ToolUser && base.pawn.Faction == Faction.OfPlayer && !base.pawn.WorkTagIsDisabled(WorkTags.Firefighting);
		if (!(flag || flag2))
		{
			return;
		}
		Fire fire = null;
		for (int i = 0; i < 9; i++)
		{
			IntVec3 c = base.pawn.Position + GenAdj.AdjacentCellsAndInside[i];
			if (!c.InBounds(base.pawn.Map))
			{
				continue;
			}
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int j = 0; j < thingList.Count; j++)
			{
				if (flag && thingList[j] is Pawn { Downed: false } pawn && base.pawn.HostileTo(pawn) && GenHostility.IsActiveThreatTo(pawn, base.pawn.Faction))
				{
					base.pawn.meleeVerbs.TryMeleeAttack(pawn);
					collideWithPawns = true;
					return;
				}
				if (flag2 && thingList[j] is Fire fire2 && (fire == null || fire2.fireSize < fire.fireSize || i == 8) && (fire2.parent == null || fire2.parent != base.pawn))
				{
					fire = fire2;
				}
			}
		}
		if (fire != null && (!base.pawn.InMentalState || base.pawn.MentalState.def.allowBeatfire))
		{
			base.pawn.natives.TryBeatFire(fire);
		}
		else
		{
			if (!flag || !job.canUseRangedWeapon || base.pawn.Faction == null || job.def != JobDefOf.Wait_Combat || (base.pawn.drafter != null && !base.pawn.drafter.FireAtWill))
			{
				return;
			}
			Verb currentEffectiveVerb = base.pawn.CurrentEffectiveVerb;
			if (currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.IsMeleeAttack)
			{
				TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToAll | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable;
				if (currentEffectiveVerb.IsIncendiary_Ranged())
				{
					targetScanFlags |= TargetScanFlags.NeedNonBurning;
				}
				Thing thing = (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(base.pawn, targetScanFlags);
				if (thing != null)
				{
					base.pawn.TryStartAttack(thing);
					collideWithPawns = true;
				}
			}
		}
	}
}
