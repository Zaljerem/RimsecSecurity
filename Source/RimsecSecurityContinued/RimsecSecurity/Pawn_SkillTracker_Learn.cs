using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Pawn_SkillTracker), "Learn")]
public class Pawn_SkillTracker_Learn
{
	public static bool Prefix(Pawn ___pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			return false;
		}
		return true;
	}
}
