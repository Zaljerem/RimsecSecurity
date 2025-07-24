using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations")]
public class PawnDiedOrDownedThoughtsUtility_AppendThoughts_Relations
{
	public static bool Prefix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(victim))
		{
			return true;
		}
		return false;
	}
}
