using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlienRace;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

public class PeacekeeperUtility
{
	public static bool IsPeacekeeper(Pawn pawn)
	{
		return pawn?.def.HasModExtension<RSPeacekeeperModExt>() ?? false;
	}

	public static Thing GetEmptyChargeStation(Pawn pawn)
	{
		if (pawn.Faction == Faction.OfPlayerSilentFail)
		{
			return (from x in pawn.Map?.listerBuildings.allBuildingsColonist.OfType<Building_ChargeStation>()
				where (x.CurrentRobot == null || x.CurrentRobot == pawn) && x.def == pawn.def?.GetModExtension<RSPeacekeeperModExt>()?.stationDef && pawn.Map.reservationManager.CanReserve(pawn, x)
				select x into station
				orderby pawn.Position.DistanceTo(station.Position)
				select station).FirstOrDefault();
		}
		return null;
	}

	public static bool IsInChargeStation(Pawn pawn)
	{
		if (pawn != null && pawn.Map != null)
		{
			return pawn.Position.GetThingList(pawn.Map).Any((Thing x) => x.def == pawn.def.GetModExtension<RSPeacekeeperModExt>().stationDef && ((Building_ChargeStation)x).CurrentRobot == pawn);
		}
		return false;
	}

	public static bool IsChargeStationFree(Thing station)
	{
		return station.Map.reservationManager.IsReservedByAnyoneOf(station, Faction.OfPlayer);
	}

	public static Pawn GetCurrentPawn(Thing pawn)
	{
		return pawn.Position.GetFirstPawn(pawn.Map) ?? PositionAbove(pawn).GetFirstPawn(pawn.Map);
	}

	public static IntVec3 PositionAbove(Thing thing)
	{
		return new IntVec3(thing.Position.x, thing.Position.y, thing.Position.z + 1);
	}

    public static Pawn GeneratePeacekeeper(PawnKindDef pawnKind, PlanetTile tile)
    {
        //Log.Message("GeneratePeacekeeper: Starting generation for " + pawnKind.defName);

        PawnGenerationRequest request = new PawnGenerationRequest(pawnKind);
        request.Faction = Faction.OfPlayer;
        request.Context = PawnGenerationContext.NonPlayer;
        request.Tile = tile;
        request.FixedBiologicalAge = 0f;
        request.FixedGender = Gender.Male;
        request.AllowAddictions = false;
        request.CanGeneratePawnRelations = false;
        request.FixedIdeo = null;
        request.ForceNoIdeo = true;
        request.ForcedXenogenes = null;
        request.ForcedEndogenes = null;

        Pawn pawn = PawnGenerator.GeneratePawn(request);

        if (pawn == null)
        {
            Log.Error("GeneratePeacekeeper: Generated pawn is null.");
            return null;
        }

        pawn.genes = null;
        pawn.ideo = null;


        //bool isExcluded = roguePawnKinds.Contains(pawnKind);
        //if (isExcluded)
        //{
            // do nothing, use the race namemaker            
        //}
        //else
        //{
            //only name legit models with the scheme, rogue models have their own namemaker
            pawn.Name = new NameSingle(pawn.Name.ToStringShort + " #" + ModSettings.peacekeeperNumber++);
        //}

        //if (pawnKind == RSDefOf.RSPeacekeeperDefenderRoguePawnKind)
        //{
            // add the explosive payload
        //    Hediff hediffBoom = HediffMaker.MakeHediff(RSDefOf.RSRogueExplosivePayload, pawn);
        //    pawn.health.AddHediff(hediffBoom, GetTorso(pawn));
       // }        

        //Log.Message("GeneratePeacekeeper: Adding hediffs for " + pawn.Name.ToStringShort);

        Hediff hediff = HediffMaker.MakeHediff(RSDefOf.RSRobotConsciousness, pawn);
        if (!pawn.health.hediffSet.HasHediff(RSDefOf.RSRobotConsciousness))
        {
            //Log.Message("GeneratePeacekeeper: Hediff1");
            pawn.health.AddHediff(hediff, pawn.health.hediffSet.GetBrain());
        }
        //Log.Message("GeneratePeacekeeper: Added RSRobotConsciousness to " + pawn.Name.ToStringShort);
        //Log.Message("GeneratePeacekeeper: Get mod extension for " + pawn.Name.ToStringShort);
        RSPeacekeeperModExt modExtension = pawn.def.GetModExtension<RSPeacekeeperModExt>();
        if (modExtension == null)
        {
            Log.Error("GeneratePeacekeeper: modExtension is null for pawn.def " + pawn.def.defName);
            return null;
        }
        //Log.Message("GeneratePeacekeeper: Got mod extension! - " + pawn.Name.ToStringShort);
        Hediff hediff2 = HediffMaker.MakeHediff(RSDefOf.RSPeacekeeperBattery, pawn);
        //Log.Message("GeneratePeacekeeper: Hediff2");
        pawn.health.AddHediff(hediff2, GetTorso(pawn));
        hediff2.Severity = modExtension.batterySeverity;
        //Log.Message("GeneratePeacekeeper: Added RSPeacekeeperBattery to " + pawn.Name.ToStringShort);
        //Log.Message("GeneratePeacekeeper: Traits for " + pawn.Name.ToStringShort);
        //if (pawn.story?.traits?.allTraits != null)
        //{
        //    List<TraitWithDegree> list = (pawn.def as ThingDef_AlienRace)?.alienRace?.generalSettings?.forcedRaceTraitEntries?.Select(entry => entry.entry)?.ToList();
        //    Log.Message("GeneratePeacekeeper: Forced trait list: " + list);
        //    if (list != null && list.Count > 0)
        //    {
        //        foreach (Trait item in pawn.story.traits.allTraits.ToList())
        //        {
        //            Log.Message("GeneratePeacekeeper: Remove item: " + item);
        //            pawn.story.traits.allTraits.Remove(item);
        //        }
        //        foreach (TraitWithDegree item2 in list)
        //        {
        //            Log.Message("GeneratePeacekeeper: Add item: " + item2);
        //            pawn.story.traits.GainTrait(new Trait(item2.def));
        //        }
        //    }
      //  }
      //  else
      //  {
      //      Log.Warning("GeneratePeacekeeper: Trait list is null for pawn " + pawn.Name.ToStringShort);
     //   }
     //   Log.Message("GeneratePeacekeeper: Finished traits for " + pawn.Name.ToStringShort);

        pawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
        pawn.skills.skills.FirstOrDefault((SkillRecord x) => x.def == SkillDefOf.Shooting).Level = modExtension.shootingSkill;
        pawn.skills.skills.FirstOrDefault((SkillRecord x) => x.def == SkillDefOf.Melee).Level = modExtension.meleeSkill;
        pawn.guest.joinStatus = JoinStatus.JoinAsColonist;

        //Log.Message("GeneratePeacekeeper: Finished generation for " + pawn.Name.ToStringShort);

        return pawn;
    }

    internal static void SpawnRandomRobot(bool defenderSecurity = false)
    {
        //Log.Message("SpawnRandomRobot: Starting");

        if (Find.World == null)
        {
            Messages.Message(new Message("No world found", MessageTypeDefOf.NegativeEvent));
            Log.Error("SpawnRandomRobot: No world found.");
            return;
        }

        Map currentMap = Find.CurrentMap ?? Find.RandomPlayerHomeMap;
        if (currentMap == null)
        {
            Messages.Message(new Message("No map found", MessageTypeDefOf.NegativeEvent));
            Log.Error("SpawnRandomRobot: No map found.");
            return;
        }

        IEnumerable<PawnKindDef> enumerable = defenderSecurity
            ? DefDatabase<PawnKindDef>.AllDefs.Where(def => def.race.HasModExtension<RSPeacekeeperModExt>() && def.race.defName == "RSPeacekeeperDefender")
            : DefDatabase<PawnKindDef>.AllDefs.Where(def => def.race.HasModExtension<RSPeacekeeperModExt>());

        if (enumerable == null || !enumerable.Any())
        {
            Log.Warning("SpawnRandomRobot: No valid PawnKindDef found.");
            return;
        }

        PawnKindDef pawnKindDef = enumerable.RandomElement();
       // Log.Message("SpawnRandomRobot: Selected PawnKindDef " + pawnKindDef.defName);

        Pawn pawn = GeneratePeacekeeper(pawnKindDef, currentMap.Tile);
        if (pawn == null)
        {
            Log.Error("SpawnRandomRobot: Generated pawn is null.");
            return;
        }

        IntVec3 intVec = currentMap.mapPawns.FreeColonists.FirstOrDefault()?.Position ?? currentMap.AllCells.Where(curCell => curCell.Walkable(currentMap) && !curCell.Fogged(currentMap) && curCell != default(IntVec3)).RandomElement();

        if (intVec == default(IntVec3))
        {
            Log.Error("SpawnRandomRobot: Unable to find a valid position to spawn the pawn.");
            return;
        }

        //Log.Message("SpawnRandomRobot: Spawning pawn at " + intVec);

        if (GenSpawn.Spawn(pawn, intVec, currentMap) is Pawn pawn2)
        {
            ThingWithComps thingWithComps = ThingMaker.MakeThing(pawnKindDef.race.GetModExtension<RSPeacekeeperModExt>().gunDef) as ThingWithComps;
            if (thingWithComps != null)
            {
                pawn2.equipment.MakeRoomFor(thingWithComps);
                pawn2.equipment.AddEquipment(thingWithComps);
            }
            else
            {
                Log.Warning("SpawnRandomRobot: Failed to create equipment for " + pawn2.Name.ToStringShort);
            }
        }

       // Log.Message("SpawnRandomRobot: Finished");
    }


    internal static void SpawnRogueRobot()
    {
        //Log.Message("SpawnRogueRobot: Starting");

        if (Find.World == null)
        {
            Messages.Message(new Message("No world found", MessageTypeDefOf.NegativeEvent));
            Log.Error("SpawnRogueRobot: No world found.");
            return;
        }

        Map currentMap = Find.CurrentMap ?? Find.RandomPlayerHomeMap;
        if (currentMap == null)
        {
            Messages.Message(new Message("No map found", MessageTypeDefOf.NegativeEvent));
            Log.Error("SpawnRogueRobot: No map found.");
            return;
        }

        IEnumerable<PawnKindDef> enumerable = new List<PawnKindDef>
{
    RSDefOf.RSPeacekeeperDefenderPawnKind,
    RSDefOf.RSPeacekeeperEnforcerPawnKind,
    RSDefOf.RSPeacekeeperSentinelPawnKind
};

        if (enumerable == null || !enumerable.Any())
        {
            Log.Warning("SpawnRogueRobot: No valid PawnKindDef found.");
            return;
        }

        PawnKindDef pawnKindDef = enumerable.RandomElement();
        // Log.Message("SpawnRandomRobot: Selected PawnKindDef " + pawnKindDef.defName);

        Pawn pawn = GeneratePeacekeeper(pawnKindDef, currentMap.Tile);
        if (pawn == null)
        {
            Log.Error("SpawnRogueRobot: Generated pawn is null.");
            return;
        }

        IntVec3 intVec = currentMap.mapPawns.FreeColonists.FirstOrDefault()?.Position ?? currentMap.AllCells.Where(curCell => curCell.Walkable(currentMap) && !curCell.Fogged(currentMap) && curCell != default(IntVec3)).RandomElement();

        if (intVec == default(IntVec3))
        {
            Log.Error("SpawnRogueRobot: Unable to find a valid position to spawn the pawn.");
            return;
        }

        //Log.Message("SpawnRandomRobot: Spawning pawn at " + intVec);

        if (GenSpawn.Spawn(pawn, intVec, currentMap) is Pawn pawn2)
        {
            ThingWithComps thingWithComps = ThingMaker.MakeThing(pawnKindDef.race.GetModExtension<RSPeacekeeperModExt>().gunDef) as ThingWithComps;
            if (thingWithComps != null)
            {
                pawn2.equipment.MakeRoomFor(thingWithComps);
                pawn2.equipment.AddEquipment(thingWithComps);
            }
            else
            {
                Log.Warning("SpawnRogueRobot: Failed to create equipment for " + pawn2.Name.ToStringShort);
            }
        }

        // Log.Message("SpawnRogueRobot: Finished");
    }



    public static IntVec3 GetSleepingPosForChargeStation(Pawn takee, Thing station)
	{
		return BedUtility.GetSleepingSlotPos(0, station.Position, station.Rotation, station.def.size);
	}

	internal static IEnumerable<BodyPartRecord> GetLegs(Pawn pawn)
	{
		foreach (BodyPartRecord notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
		{
			if (notMissingPart.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
			{
				yield return notMissingPart;
			}
		}
	}

	public static BodyPartRecord GetTorso(Pawn pawn)
	{
		foreach (BodyPartRecord notMissingPart in pawn.health.hediffSet.GetNotMissingParts())
		{
			if (notMissingPart.def == RSDefOf.MechanicalThorax)
			{
				return notMissingPart;
			}
		}
		return null;
	}

	internal static void RefuelPawnOnCaravan(Pawn pawn, Caravan caravan)
	{
		if ((double)pawn.needs.rest.CurLevel > 0.4)
		{
			return;
		}
		Thing thing = caravan.AllThings.FirstOrDefault((Thing t) => t.def == RSDefOf.RSPowerCell);
		if (thing != null)
		{
			Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, thing);
			int num = Math.Min(thing.stackCount, 6);
			if (thing.stackCount > num)
			{
				thing.stackCount -= num;
			}
			else
			{
				ownerOf.inventory.innerContainer.Remove(thing);
			}
			pawn.needs.rest.CurLevel += (float)num / 10f;
		}
	}

	public static Job RefuelJob(Pawn pawn, Thing t, bool forced = false, JobDef customRefuelJob = null)
	{
		Thing thing = FindBestFuel(pawn);
		return JobMaker.MakeJob(customRefuelJob ?? RSDefOf.RSFuelRobot, t, thing);
	}

	public static Thing FindBestFuel(Pawn pawn)
	{
		ThingFilter filter = new ThingFilter();
		filter.SetAllow(RSDefOf.RSPowerCell, allow: true);
		return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, filter.BestThingRequest, PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
		bool validator(Thing x)
		{
			if (!x.IsForbidden(pawn) && pawn.CanReserve(x))
			{
				return filter.Allows(x);
			}
			return false;
		}
	}

	public static void RunSavely(Action action)
	{
		RunSavely(delegate
		{
			action();
			return 0;
		});
	}

	public static T RunSavely<T>(Func<T> action)
	{
		try
		{
			return action();
		}
		catch (Exception ex)
		{
			Log.Error(ex.ToString());
		}
		return default(T);
	}

	public static Assembly GetAssemblyFromString(string assemblyName)
	{
		return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly assembly) => assembly.FullName.ToLower().Contains(assemblyName));
	}

    // Define a list of PawnKindDefs to exclude
  // public static List<PawnKindDef> roguePawnKinds = new List<PawnKindDef>
//{
  //  RSDefOf.RSPeacekeeperDefenderRoguePawnKind,
  //  RSDefOf.RSPeacekeeperEnforcerRoguePawnKind,
  //   RSDefOf.RSPeacekeeperSentinelRoguePawnKind

    // Add more PawnKindDefs to exclude
//};


}
