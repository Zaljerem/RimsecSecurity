using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimsecSecurity;

internal class HediffComp_RobotConsciousness : HediffComp
{
	public List<TerrainDef> allowedTerrain = new List<TerrainDef> { TerrainDefOf.Ice };

	public List<TerrainDef> allowedTerrainSand = new List<TerrainDef>
	{
		TerrainDefOf.Sand,
		TerrainDef.Named("SoftSand")
	};

	public List<TerrainDef> allowedTerrainForest = new List<TerrainDef>
	{
		TerrainDefOf.Soil,
		TerrainDefOf.Gravel
	};

	public List<BiomeDef> allowedBiomes = new List<BiomeDef>
	{
		BiomeDefOf.SeaIce,
		BiomeDefOf.IceSheet
	};

	public List<BiomeDef> allowedBiomesSand = new List<BiomeDef>
	{
		BiomeDefOf.Desert,
		BiomeDefOf.Tundra
	};

	public List<BiomeDef> allowedBiomesForest = new List<BiomeDef>
	{
		BiomeDefOf.BorealForest,
		BiomeDefOf.TemperateForest
	};

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (parent?.pawn != null && parent.pawn.Tile != -1 && (Find.TickManager.TicksGame + parent.pawn.thingIDNumber) % 120 == 0)
		{
			CheckTerrain();
		}
	}

	private void CheckTerrain()
	{
		Trait trait2 = parent.pawn.story.traits.allTraits.FirstOrDefault((Trait trait) => trait.def == RSDefOf.RSTraitWinter || trait.def == RSDefOf.RSTraitDesert || trait.def == RSDefOf.RSTraitForest);
		if (trait2 == null)
		{
			return;
		}

        Tile tile = Find.WorldGrid[parent.pawn.Tile];

        // Use reflection to get the private field "biome"
        FieldInfo biomeField = typeof(Tile).GetField("biome", BindingFlags.NonPublic | BindingFlags.Instance);
        BiomeDef biome = biomeField?.GetValue(tile) as BiomeDef;


        bool flag = false;
		List<TerrainDef> list = ((trait2.def == RSDefOf.RSTraitWinter) ? allowedTerrain : allowedTerrainSand);
		List<BiomeDef> list2 = ((trait2.def == RSDefOf.RSTraitWinter) ? allowedBiomes : allowedBiomesSand);
		if (trait2.def == RSDefOf.RSTraitForest)
		{
			if ((parent.pawn.Map == null && allowedBiomesForest.Contains(biome)) || (parent.pawn.Map != null && allowedBiomesForest.Contains(biome) && allowedTerrainForest.Contains(parent.pawn.Position.GetTerrain(parent.pawn.Map))))
            {
				flag = true;
			}
		}
		else if ((parent.pawn.Map == null && list2.Contains(biome)) || (parent.pawn.Map != null && (list.Contains(parent.pawn.Position.GetTerrain(parent.pawn.Map)) || (trait2.def == RSDefOf.RSTraitWinter && parent.pawn.Map.snowGrid.TotalDepth > 100f))))
		{
			flag = true;
		}
		if (flag)
		{
			if (parent.pawn.health.hediffSet.HasHediff(RSDefOf.RSTerrainAdvantage))
			{
				return;
			}
			{
				foreach (BodyPartRecord leg in PeacekeeperUtility.GetLegs(parent.pawn))
				{
					Hediff hediff2 = HediffMaker.MakeHediff(RSDefOf.RSTerrainAdvantage, parent.pawn);
					parent.pawn.health.AddHediff(hediff2, leg);
				}
				return;
			}
		}
		if (parent.pawn.health.hediffSet.HasHediff(RSDefOf.RSTerrainAdvantage))
		{
			parent.pawn.health.hediffSet.hediffs.RemoveAll((Hediff hediff) => hediff.def == RSDefOf.RSTerrainAdvantage);
		}
	}
}
