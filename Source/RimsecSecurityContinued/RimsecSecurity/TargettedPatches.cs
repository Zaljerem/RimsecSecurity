using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimsecSecurity;

public class TargettedPatches
{
	public static void ColonistBar_CheckRecacheEntries_Postfix(List<ColonistBar.Entry> ___cachedEntries)
	{
		___cachedEntries.RemoveAll((ColonistBar.Entry entry) => PeacekeeperUtility.IsPeacekeeper(entry.pawn));
	}

	public static void StorytellerUtilityPopulation_AdjustedPopulation_get_Postfix(ref float __result)
	{
		int? num = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists?.Where((Pawn colonist) => PeacekeeperUtility.IsPeacekeeper(colonist))?.Count();
		if (num.HasValue)
		{
			__result -= num.Value;
		}
	}
}
