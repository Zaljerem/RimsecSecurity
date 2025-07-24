using Verse;

namespace RimsecSecurity;

internal class RSPeacekeeperModExt : DefModExtension
{
	public bool isPeacekeeper = true;

	public ThingDef stationDef;

	public int stationZOffset;

	public int repairTicks = 400;

	public ThingDef gunDef;

	public float batterySeverity;

	public int meleeSkill = 10;

	public int shootingSkill = 10;
}
