using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal class CompRechargeRobot : ThingComp
{
	private int ticksCharge = -1;

	private int ticksHeal = -1;

	private int ticksHealPermanent = -1;

	private int ticksRestorePart = -1;

	private float componentsForManualRepair;

	private float availableComponents;

	private Building_ChargeStation cachedParent;

	public CompProperties_RechargeRobot Props => (CompProperties_RechargeRobot)props;

	public float ComponentsForManualRepair
	{
		get
		{
			return componentsForManualRepair;
		}
		set
		{
			componentsForManualRepair = value;
		}
	}

	public float AvailableComponents
	{
		get
		{
			return availableComponents;
		}
		set
		{
			availableComponents = value;
		}
	}

	public Building_ChargeStation Parent => cachedParent ?? (cachedParent = parent as Building_ChargeStation);

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_Values.Look(ref ticksCharge, "ticksCharge", -1);
		Scribe_Values.Look(ref ticksHeal, "ticksHeal", -1);
		Scribe_Values.Look(ref ticksHealPermanent, "ticksHealPermanent", -1);
		Scribe_Values.Look(ref ticksRestorePart, "ticksRestorePart", -1);
		Scribe_Values.Look(ref componentsForManualRepair, "componentsForManualRepair", 0f);
		Scribe_Values.Look(ref availableComponents, "availableComponents", 0f);
	}

	public override void CompTick()
	{
		if (Parent == null || Parent.PowerOff())
		{
			return;
		}
		if (ticksCharge >= 60)
		{
			if (RobotTreatable())
			{
				ComponentsForManualRepair = CalculateManualRepairCost();
				AvailableComponents = Parent.CompFuel.Fuel;
				Parent.CurrentRobot.needs.rest.CurLevel = ((Parent.CurrentRobot.needs.rest.CurLevel + Props.energyPerSecond > Parent.CurrentRobot.needs.rest.MaxLevel) ? Parent.CurrentRobot.needs.rest.MaxLevel : (Parent.CurrentRobot.needs.rest.CurLevel + Props.energyPerSecond));
			}
			ticksCharge = 0;
		}
		if (ticksHeal >= 1799)
		{
			Building_ChargeStation building_ChargeStation = Parent;
			if (building_ChargeStation != null && (building_ChargeStation.CompFuel?.HasFuel).GetValueOrDefault())
			{
				if (RobotTreatable())
				{
					bool flag = false;
					int num = 0;
					foreach (Hediff hediff3 in Parent.CurrentRobot.health.hediffSet.hediffs)
					{
						if (hediff3.def == RSDefOf.RSRobotConsciousness)
						{
							flag = true;
						}
						if (!(hediff3 is Hediff_Injury hediff_Injury) || hediff_Injury.IsPermanent())
						{
							continue;
						}
						if (hediff_Injury.IsTended())
						{
							hediff_Injury.Heal(Props.injuryHealAmountPer30s);
							if (num++ > Props.injuryHealCount)
							{
								break;
							}
						}
						else
						{
							hediff_Injury.Tended(1f, 1f);
						}
					}
					if (!flag)
					{
						Hediff hediff2 = HediffMaker.MakeHediff(RSDefOf.RSRobotConsciousness, Parent.CurrentRobot);
						Parent.CurrentRobot.health.AddHediff(hediff2, Parent.CurrentRobot.health.hediffSet.GetBrain());
					}
				}
				ticksHeal = 0;
			}
		}
		if (ticksHealPermanent >= Props.ticksHealPermanent)
		{
			Building_ChargeStation building_ChargeStation2 = Parent;
			if (building_ChargeStation2 != null && (building_ChargeStation2.CompFuel?.HasFuel).GetValueOrDefault())
			{
				if (RobotTreatable())
				{
					Hediff_Injury hediff_Injury2 = Parent.CurrentRobot.health.hediffSet.hediffs?.OfType<Hediff_Injury>()?.InRandomOrder()?.FirstOrDefault((Hediff_Injury hediff) => hediff.IsPermanent());
					if (hediff_Injury2 != null)
					{
						HealthUtility.Cure(hediff_Injury2);
					}
				}
				ticksHealPermanent = 0;
			}
		}
		if (ticksRestorePart >= Props.ticksRestorePart)
		{
			Building_ChargeStation building_ChargeStation3 = Parent;
			if (building_ChargeStation3 != null && (building_ChargeStation3.CompFuel?.HasFuel).GetValueOrDefault())
			{
				if (RobotTreatable())
				{
					Hediff_MissingPart hediff_MissingPart = Parent.CurrentRobot.health.hediffSet.hediffs?.OfType<Hediff_MissingPart>()?.InRandomOrder()?.FirstOrDefault();
					if (hediff_MissingPart != null)
					{
						HealthUtility.Cure(hediff_MissingPart);
					}
				}
				ticksRestorePart = 0;
			}
		}
		ticksCharge++;
		ticksHeal++;
		ticksHealPermanent++;
		ticksRestorePart++;
	}

	private bool RobotTreatable()
	{
		if (Parent.CurrentRobot != null && !Parent.CurrentRobot.Dead)
		{
			return PeacekeeperUtility.IsPeacekeeper(Parent.CurrentRobot);
		}
		return false;
	}

	private Pawn GetCurrentPawn()
	{
		return parent.Position.GetFirstPawn(parent.Map) ?? new IntVec3(parent.Position.x, parent.Position.y, parent.Position.z + 1).GetFirstPawn(parent.Map);
	}

	private float CalculateManualRepairCost()
	{
		if (Parent?.CurrentRobot?.health?.hediffSet == null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (Hediff hediff in Parent.CurrentRobot.health.hediffSet.hediffs)
		{
			if (hediff is Hediff_Injury hediff_Injury)
			{
				num = ((!hediff_Injury.IsPermanent()) ? (num + hediff_Injury.Severity * Props.repairCostPerPointOfDamage) : (num + Props.repairCostPermanent));
			}
			else if (hediff is Hediff_MissingPart)
			{
				num += Props.repairCostMissing;
			}
		}
		if (!(num > Props.repairCostMax))
		{
			return num;
		}
		return Props.repairCostMax;
	}

	public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
	{
		foreach (FloatMenuOption item in base.CompFloatMenuOptions(selPawn))
		{
			yield return item;
		}
		if (RobotTreatable() && !Parent.PowerOff() && ComponentsForManualRepair != 0f)
		{
			AcceptanceReport acceptanceReport = CanRepairRobo(selPawn);
			TaggedString taggedString = "RSRepairManually".Translate();
			if (!acceptanceReport.Accepted && !string.IsNullOrWhiteSpace(acceptanceReport.Reason))
			{
				taggedString = taggedString + ": " + acceptanceReport.Reason;
			}
			yield return new FloatMenuOption(taggedString, delegate
			{
				RepairManually(selPawn);
			})
			{
				Disabled = !acceptanceReport.Accepted,
				revalidateClickTarget = parent
			};
		}
	}

	public AcceptanceReport CanRepairRobo(Pawn pawn)
	{
		if (pawn.Dead || pawn.Faction != Faction.OfPlayer)
		{
			return false;
		}
		if (!pawn.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
		{
			return new AcceptanceReport("can't reach");
		}
		if (!pawn.Map.reservationManager.CanReserve(pawn, Parent.CurrentRobot))
		{
			Pawn pawn2 = pawn.Map.reservationManager.FirstRespectedReserver(Parent.CurrentRobot, pawn);
			return new AcceptanceReport((pawn2 == null) ? "Reserved".Translate() : "ReservedBy".Translate(pawn.LabelShort, pawn2));
		}
		if (ComponentsForManualRepair == 0f || AvailableComponents < ComponentsForManualRepair)
		{
			return new AcceptanceReport($"{ComponentsForManualRepair} components are required for manual repairs, refill the station.");
		}
		return AcceptanceReport.WasAccepted;
	}

	public void RepairManually(Pawn pawn)
	{
		Job job = JobMaker.MakeJob(RSDefOf.RSManualRepair, parent, Parent.CurrentRobot);
		job.count = 1;
		pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
	}
}
