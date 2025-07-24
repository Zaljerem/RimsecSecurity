using Verse;

namespace RimsecSecurity;

internal class CompSpawnPeacekeeper : ThingComp
{
	public CompProperties_SpawnPeacekeeper Props => (CompProperties_SpawnPeacekeeper)props;

	public override void CompTick()
	{
		Pawn obj = GenSpawn.Spawn(PeacekeeperUtility.GeneratePeacekeeper(Props.pawnKind, parent.Tile), parent.Position, parent.Map, Rot4.South) as Pawn;
		ThingWithComps thingWithComps = ThingMaker.MakeThing(Props.weaponDef) as ThingWithComps;
		obj.equipment.MakeRoomFor(thingWithComps);
		obj.equipment.AddEquipment(thingWithComps);
		parent.Destroy();
	}
}
