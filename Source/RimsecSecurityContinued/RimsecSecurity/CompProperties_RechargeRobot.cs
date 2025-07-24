using Verse;

namespace RimsecSecurity;

internal class CompProperties_RechargeRobot : CompProperties
{
	public float energyPerSecond;

	public int injuryHealCount;

	public float injuryHealAmountPer30s;

	public int ticksHealPermanent;

	public int ticksRestorePart;

	public float repairCostPerPointOfDamage = 0.05f;

	public float repairCostPermanent = 0.5f;

	public float repairCostMissing = 1f;

	public float repairCostMax = 5f;

	public CompProperties_RechargeRobot()
	{
		compClass = typeof(CompRechargeRobot);
	}
}
