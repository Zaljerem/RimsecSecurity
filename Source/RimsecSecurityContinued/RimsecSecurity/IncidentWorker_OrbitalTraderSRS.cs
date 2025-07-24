using RimWorld;
using Verse;

namespace RimsecSecurity;

internal class IncidentWorker_OrbitalTraderSRS : IncidentWorker
{
	private const int MaxShips = 5;

	protected override bool CanFireNowSub(IncidentParms parms)
	{
		if (base.CanFireNowSub(parms))
		{
			return ((Map)parms.target).passingShipManager.passingShips.Count < 5;
		}
		return false;
	}

	protected override bool TryExecuteWorker(IncidentParms parms)
	{
		Map map = (Map)parms.target;
		if (map.passingShipManager.passingShips.Count >= 5)
		{
			return false;
		}
		TradeShip tradeShip = new TradeShip(RSDefOf.RSSRSTradeShip);
		if (map.listerBuildings.allBuildingsColonist.Any((Building b) => b.def.IsCommsConsole && (b.GetComp<CompPowerTrader>() == null || b.GetComp<CompPowerTrader>().PowerOn)))
		{
			SendStandardLetter(tradeShip.def.LabelCap, "TraderArrival".Translate(tradeShip.name, tradeShip.def.label, (tradeShip.Faction == null) ? "TraderArrivalNoFaction".Translate() : "TraderArrivalFromFaction".Translate(tradeShip.Faction.Named("FACTION"))), LetterDefOf.PositiveEvent, parms, LookTargets.Invalid);
		}
		map.passingShipManager.AddShip(tradeShip);
		tradeShip.GenerateThings();
		return true;
	}
}
