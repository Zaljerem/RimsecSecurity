using Verse;

namespace RimsecSecurity;

internal static class GuardsForMePatches
{
	public static void guardNeedFood_Postfix(ref bool __result, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}

	public static void guardNeedJoy_Postfix(ref bool __result, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}

	public static void guardNeedMood_Postfix(ref bool __result, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}

	public static void guardNeedHygiene_Postfix(ref bool __result, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}

	public static void guardNeedBladder_Postfix(ref bool __result, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}
}
