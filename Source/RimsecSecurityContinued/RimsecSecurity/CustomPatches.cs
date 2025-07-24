using System.Collections.Generic;
using System.Linq;
using AlienRace;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[StaticConstructorOnStartup]
internal static class CustomPatches
{
	static CustomPatches()
	{
		PeacekeeperUtility.RunSavely(PatchRobotNoFoodAndRecipes);
		PeacekeeperUtility.RunSavely(PatchStorytellers);
		PeacekeeperUtility.RunSavely(PatchRemoveRottingFromCorpses);
		PeacekeeperUtility.RunSavely(PatchFuelConsumption);
		PeacekeeperUtility.RunSavely(PatchRobotClothing);
	}

	private static void PatchFuelConsumption()
	{
		CompProperties_Refuelable compProperties = RSDefOf.RSChargeStation.GetCompProperties<CompProperties_Refuelable>();
		if (compProperties != null)
		{
			compProperties.fuelConsumptionRate = ModSettings.fuelConsumptionRate;
		}
	}

	public static void PatchRobotNoFoodAndRecipes()
	{
		IEnumerable<PawnKindDef> enumerable = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef def) => def != null && (def.race?.HasModExtension<RSPeacekeeperModExt>()).GetValueOrDefault());
		RecipeDef namedSilentFail = DefDatabase<RecipeDef>.GetNamedSilentFail("ButcherCorpseFlesh");
		if (namedSilentFail == null)
		{
			Log.Error("ButcherCorpseFlesh recipe not found and null");
		}
		IEnumerable<RecipeDef> enumerable2 = DefDatabase<RecipeDef>.AllDefs.Where((RecipeDef def) => def.workerClass == typeof(Recipe_AdministerIngestible));
		foreach (PawnKindDef item in enumerable)
		{
			item.RaceProps.corpseDef.ingestible.foodType = FoodTypeFlags.None;
			namedSilentFail?.fixedIngredientFilter.SetAllow(item.RaceProps.corpseDef, allow: false);
			foreach (RecipeDef item2 in enumerable2)
			{
				if (item2.recipeUsers != null)
				{
					item2.recipeUsers.Remove(item.race);
				}
			}
		}
	}

	public static void PatchRobotClothing()
	{
		if (!ModSettings.allowClothing)
		{
			return;
		}
		foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef def) => def != null && (def.race?.HasModExtension<RSPeacekeeperModExt>()).GetValueOrDefault()))
		{
			(item.race as ThingDef_AlienRace).alienRace.raceRestriction.onlyUseRaceRestrictedApparel = false;
		}
	}

	public static void PatchStorytellers()
	{
		foreach (StorytellerDef allDef in DefDatabase<StorytellerDef>.AllDefs)
		{
			allDef.comps.Add(new StorytellerCompProperties_OnOffCycle
			{
				incident = RSDefOf.RSSRSOrbitalTraderIncident,
				onDays = 2f,
				offDays = ModSettings.daysPauseBetweenTradeShips,
				numIncidentsRange = new FloatRange(1f, 1f)
			});
		}
	}

	private static void PatchRemoveRottingFromCorpses()
	{
		foreach (PawnKindDef item in DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef def) => def.race.HasModExtension<RSPeacekeeperModExt>()))
		{
			CompProperties_Rottable compProperties = item.RaceProps.corpseDef.GetCompProperties<CompProperties_Rottable>();
			if (compProperties != null)
			{
				item.RaceProps.corpseDef.comps.Remove(compProperties);
			}
		}
	}
}
