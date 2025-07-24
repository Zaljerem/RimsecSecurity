using Verse;

namespace RimsecSecurity;


//no longer working. Rework for 1.6
internal static class SaveOurShip2Patches
{
	public static void ShipInteriorMod2_hasSpaceSuit_Postfix(ref bool __result, Pawn pawn)
	{
		if (!__result && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = true;
		}
	}

	public static void ShipInteriorMod2_EVAlevel_Postfix(ref byte __result, Pawn pawn)
	{
		if (__result != 8 && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = 8;
		}
	}
}
