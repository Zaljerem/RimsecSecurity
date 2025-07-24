using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Need), "get_LabelCap")]
public class Need_get_LabelCap
{
	public static void Postfix(ref string __result, Need __instance, Pawn ___pawn)
	{
		if (__instance.def.defName == "Rest" && PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			__result = "RSEnergyLabel".Translate();
		}
		else if (new string[5] { "Joy", "Comfort", "Beauty", "Outdoors", "Mood" }.Contains(__instance.def.defName) && PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			__result = string.Empty;
		}
	}
}
