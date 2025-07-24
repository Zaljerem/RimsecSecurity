using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
public class RestUtility_IsValidBedFor
{
	public static void Postfix(ref bool __result, Thing bedThing, Pawn sleeper)
	{
		if (PeacekeeperUtility.IsPeacekeeper(sleeper) && bedThing.def == RSDefOf.RSChargeStation)
		{
			__result = true;
		}
		else if (PeacekeeperUtility.IsPeacekeeper(sleeper) && bedThing.def != RSDefOf.RSChargeStation)
		{
			__result = false;
		}
		else if (bedThing.def == RSDefOf.RSChargeStation)
		{
			__result = false;
		}
	}
}
