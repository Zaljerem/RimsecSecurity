using RimWorld;
using Verse;

namespace RimsecSecurity;

internal class StockGenerator_SRSGuns : StockGenerator_MiscItems
{
	private static readonly SimpleCurve SelectionWeightMarketValueCurve = new SimpleCurve
	{
		new CurvePoint(0f, 1f),
		new CurvePoint(1000f, 1f),
		new CurvePoint(2000f, 0.7f),
		new CurvePoint(4000f, 0.5f)
	};

	public const string weaponTag = "RSPeacekeeperGun";

	protected override float SelectionWeight(ThingDef thingDef)
	{
		return SelectionWeightMarketValueCurve.Evaluate(thingDef.BaseMarketValue);
	}

	public override bool HandlesThingDef(ThingDef td)
	{
		if (td.IsRangedWeapon)
		{
			if (td.weaponTags != null)
			{
				return td.weaponTags.Contains("RSPeacekeeperGun");
			}
			return false;
		}
		return false;
	}
}
