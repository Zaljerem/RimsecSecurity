using System;
using UnityEngine;
using Verse;

namespace RimsecSecurity;

internal class ModSettings : Verse.ModSettings
{
	public static int peacekeeperNumber = 1;

	public static bool butcheredPeacekeeper;

	public static float energyShortageSeverityMult = 10f;

	public static bool hidePeacekeepersFromColonistBar = false;

	public static bool debugActive = false;

	public static bool countPeacekeepersTowardsPopulation = true;

	public static float fuelConsumptionRate;

	public static float daysPauseBetweenTradeShips = 15f;

	public static bool allowClothing = false;

	public static bool removeIdeologyImpact = true;

	public static bool rogueCharger = false;

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref peacekeeperNumber, "peacekeeperNumber", 1);
		Scribe_Values.Look(ref energyShortageSeverityMult, "energyShortageSeverityMult", 0f);
		Scribe_Values.Look(ref hidePeacekeepersFromColonistBar, "hidePeacekeepersFromColonistBar", defaultValue: false);
		Scribe_Values.Look(ref countPeacekeepersTowardsPopulation, "countPeacekeepersTowardsPopulation", defaultValue: true);
		Scribe_Values.Look(ref fuelConsumptionRate, "fuelConsumptionRate", 0.5f);
		Scribe_Values.Look(ref debugActive, "debugActive", defaultValue: false);
		Scribe_Values.Look(ref daysPauseBetweenTradeShips, "daysPauseBetweenTradeShips", 15f);
		Scribe_Values.Look(ref allowClothing, "allowClothing", defaultValue: false);
		Scribe_Values.Look(ref removeIdeologyImpact, "removeIdeologyImpact", defaultValue: true);
        Scribe_Values.Look(ref rogueCharger, "rogueCharger", defaultValue: false);
    }

	public void DoWindowContents(Rect rect)
	{
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(rect);
		listing_Standard.Label("RS_RequireRestart".Translate());
		listing_Standard.Gap(24f);
		listing_Standard.CheckboxLabeled("RS_HideFromColonistBar".Translate(), ref hidePeacekeepersFromColonistBar);
		listing_Standard.CheckboxLabeled("RS_CountRobotsForPop".Translate(), ref countPeacekeepersTowardsPopulation);
		listing_Standard.CheckboxLabeled("RS_AllowClothes".Translate(), ref allowClothing);
		listing_Standard.Gap(24f);
		listing_Standard.Label("RS_MaintStationFuelConsump".Translate() + $"{Math.Round(fuelConsumptionRate, 2)}");
		fuelConsumptionRate = listing_Standard.Slider(fuelConsumptionRate, 0.01f, 2f);
		listing_Standard.Label("RS_TradeShipIntervalDays".Translate() + $"{Math.Round(daysPauseBetweenTradeShips, 1)}");
		daysPauseBetweenTradeShips = listing_Standard.Slider(daysPauseBetweenTradeShips, 1f, 60f);
		listing_Standard.CheckboxLabeled("RS_RemoveDiversityImpact".Translate(), ref removeIdeologyImpact);
        //listing_Standard.CheckboxLabeled("RS_RogueCharger".Translate(), ref rogueCharger);
        listing_Standard.Gap(24f);
		if (listing_Standard.ButtonTextLabeled("RS_SpawnRandomRobot".Translate(), "RS_Spawn".Translate()))
		{
			PeacekeeperUtility.SpawnRandomRobot();
		}
		if (listing_Standard.ButtonTextLabeled("RS_SpawnDefenderRobot".Translate(), "RS_Spawn".Translate()))
		{
			PeacekeeperUtility.SpawnRandomRobot(defenderSecurity: true);
		}
        //if (listing_Standard.ButtonTextLabeled("RS_SpawnRogueRobot".Translate(), "RS_RogueSpawn".Translate()))
       // {
        //    PeacekeeperUtility.SpawnRogueRobot();
       // }
        listing_Standard.End();
	}
}
