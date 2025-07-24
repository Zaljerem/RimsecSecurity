using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimsecSecurity;

internal class StockGenerator_SRSRobots : StockGenerator
{
	private static readonly SimpleCurve SelectionWeightMarketValueCurve = new SimpleCurve
	{
		new CurvePoint(0f, 1f),
		new CurvePoint(500f, 1f),
		new CurvePoint(1500f, 0.7f),
		new CurvePoint(5000f, 0.5f)
	};

	public override IEnumerable<Thing> GenerateThings(PlanetTile forTile, Faction faction = null)
	{
		int count = countRange.RandomInRange;        

        // Filter PawnKindDefs to include only those with the mod extension and not in the excluded list
        IEnumerable<PawnKindDef> validPawnKinds = DefDatabase<PawnKindDef>.AllDefsListForReading
            .Where((PawnKindDef def) =>
                def.race.HasModExtension<RSPeacekeeperModExt>());


		//.Where((PawnKindDef def) =>
         //       def.race.HasModExtension<RSPeacekeeperModExt>() &&
         //       !PeacekeeperUtility.roguePawnKinds.Contains(def));


        //IEnumerable<PawnKindDef> validPawnKinds = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef def) => def.race.HasModExtension<RSPeacekeeperModExt>());
        if (validPawnKinds == null || validPawnKinds.Count() == 0)
		{
			yield break;
		}
		for (int i = 0; i < count; i++)
		{
			validPawnKinds.TryRandomElementByWeight(SelectionWeight, out var result);
			Pawn pawn = PeacekeeperUtility.GeneratePeacekeeper(result, forTile);
			if (pawn != null)
			{
				pawn.SetFaction(Faction.OfEmpire);
				if (ThingMaker.MakeThing(result.race.GetModExtension<RSPeacekeeperModExt>().gunDef) is ThingWithComps thingWithComps)
				{
					pawn.equipment.MakeRoomFor(thingWithComps);
					pawn.equipment.AddEquipment(thingWithComps);
				}
				yield return pawn;
			}
		}
	}

	protected float SelectionWeight(PawnKindDef thingDef)
	{
		return SelectionWeightMarketValueCurve.Evaluate(thingDef.race.BaseMarketValue);
	}

	public override bool HandlesThingDef(ThingDef thingDef)
	{
		if (thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike)
		{
			return (int)thingDef.tradeability > 0;
		}
		return false;
	}
}
