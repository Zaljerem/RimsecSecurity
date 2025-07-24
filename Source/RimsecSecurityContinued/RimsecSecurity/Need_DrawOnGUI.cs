using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Need), "DrawOnGUI")]
public class Need_DrawOnGUI
{
	public static bool Prefix(Need __instance, Pawn ___pawn)
	{
		if (!new string[5] { "Joy", "Comfort", "Beauty", "Outdoors", "Mood" }.Contains(__instance.def.defName) || !PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			return true;
		}
		return false;
	}
}
