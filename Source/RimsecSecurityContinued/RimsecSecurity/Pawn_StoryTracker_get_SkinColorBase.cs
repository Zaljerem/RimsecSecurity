using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Pawn_StoryTracker), "get_SkinColorBase")]
public class Pawn_StoryTracker_get_SkinColorBase
{
	public static bool Prefix(ref Color __result, Pawn_StoryTracker __instance, Pawn ___pawn)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			return true;
		}
		__result = Color.black;
		return false;
	}
}
