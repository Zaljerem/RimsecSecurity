using Verse;
using Verse.AI;

namespace RimsecSecurity;

internal static class PrisonLaberPatches
{
	public static void JobGiver_Bedtime_TryGiveJob_Postfix(ref Job __result, Pawn pawn)
	{
		if (__result != null && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = null;
		}
	}

	public static void JobGiver_Diet_TryGiveJob_Postfix(ref Job __result, Pawn pawn)
	{
		if (__result != null && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = null;
		}
	}

	public static bool JobGiver_Labor_TryIssueJobPackage_Prefix(ref ThinkResult __result, Pawn pawn)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			return true;
		}
		__result = ThinkResult.NoJob;
		return false;
	}

	public static bool WorkSettings_InitWorkSettings_Prefix(Pawn pawn)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			return true;
		}
		return false;
	}

	public static bool Need_Treatment_NeedInterval_Prefix(Pawn ___pawn)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			return true;
		}
		return false;
	}
}
