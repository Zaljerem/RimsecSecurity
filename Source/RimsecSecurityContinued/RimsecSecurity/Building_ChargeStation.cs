using RimWorld;
using Verse;

namespace RimsecSecurity;

internal class Building_ChargeStation : Building
{
	private Graphic graphicStationOn;

	private CompFlickable compFlick;

	private CompPowerTrader compPower;

	private CompRefuelable compFuel;

	private CompRechargeRobot compRecharge;

	private Pawn currentRobot;

	public CompFlickable CompFlick
	{
		get
		{
			return compFlick ?? (compFlick = this.TryGetComp<CompFlickable>());
		}
		set
		{
			compFlick = value;
		}
	}

	public CompPowerTrader CompPower
	{
		get
		{
			return compPower ?? (compPower = this.TryGetComp<CompPowerTrader>());
		}
		set
		{
			compPower = value;
		}
	}

	public CompRefuelable CompFuel
	{
		get
		{
			return compFuel ?? (compFuel = this.TryGetComp<CompRefuelable>());
		}
		set
		{
			compFuel = value;
		}
	}

	public CompRechargeRobot CompRecharge
	{
		get
		{
			return compRecharge ?? (compRecharge = this.TryGetComp<CompRechargeRobot>());
		}
		set
		{
			compRecharge = value;
		}
	}

	public Pawn CurrentRobot
	{
		get
		{
			return currentRobot;
		}
		set
		{
			currentRobot = value;
		}
	}

	public override Graphic Graphic
	{
		get
		{
			if (this == null)
			{
				Log.Message("this is null");
				return null;
			}
			if (PowerOff() || CompFlick == null || !CompFlick.SwitchIsOn)
			{
				return base.DefaultGraphic;
			}
			if (graphicStationOn == null)
			{
				if (ModSettings.rogueCharger)
				{
                    graphicStationOn = GraphicDatabase.Get(def.graphicData.graphicClass, def.graphicData.texPath + "_Rogue", def.graphicData.shaderType.Shader, def.graphicData.drawSize, DrawColor, DrawColorTwo);
                }
				else
				{
                    graphicStationOn = GraphicDatabase.Get(def.graphicData.graphicClass, def.graphicData.texPath + "_On", def.graphicData.shaderType.Shader, def.graphicData.drawSize, DrawColor, DrawColorTwo);
                }				
				GraphicData data = def.graphic.data;
				graphicStationOn.data = data;
			}
			return graphicStationOn;
		}
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_References.Look(ref currentRobot, "currentRobot");
	}

	protected override void Tick()
	{
		base.Tick();
		if ((Find.TickManager.TicksGame + thingIDNumber) % 60 == 0 && CurrentRobot != null && (CurrentRobot?.Map != base.Map || (CurrentRobot.Position != base.Position && CurrentRobot.Position != PeacekeeperUtility.PositionAbove(this))))
		{
			CurrentRobot = null;
		}
	}

	public bool PowerOff()
	{
		if (CompPower != null && CompPower.PowerOn)
		{
			CompPowerTrader compPowerTrader = CompPower;
			if (compPowerTrader == null)
			{
				return true;
			}
			return !(compPowerTrader.PowerNet?.HasActivePowerSource).GetValueOrDefault();
		}
		return true;
	}

	public IntVec3 GetStandPosition(Pawn pawn)
	{
		RSPeacekeeperModExt modExtension = pawn.def.GetModExtension<RSPeacekeeperModExt>();
		if (modExtension == null)
		{
			Log.Error("pawn is not a peacekeeper");
			return default(IntVec3);
		}
		return new IntVec3(base.Position.x, base.Position.y, base.Position.z + modExtension.stationZOffset);
	}
}
