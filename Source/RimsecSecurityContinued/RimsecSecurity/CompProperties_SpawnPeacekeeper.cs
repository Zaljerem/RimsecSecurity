using Verse;

namespace RimsecSecurity;

internal class CompProperties_SpawnPeacekeeper : CompProperties
{
	public PawnKindDef pawnKind;

	public ThingDef weaponDef;

	public CompProperties_SpawnPeacekeeper()
	{
		compClass = typeof(CompSpawnPeacekeeper);
	}
}
