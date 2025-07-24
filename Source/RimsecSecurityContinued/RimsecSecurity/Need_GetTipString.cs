using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Need), "GetTipString")]
public class Need_GetTipString
{
	public static void Postfix(ref string __result, Need __instance, Pawn ___pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			if (__instance.def.defName == "Rest")
			{
				__result = __instance.LabelCap + ": " + __instance.CurLevelPercentage.ToStringPercent() + " (" + __instance.CurLevel.ToString("0.##") + " / " + __instance.MaxLevel.ToString("0.##") + ")\n" + "RSEnergyDesc".Translate();
			}
			else if (new string[5] { "Joy", "Comfort", "Beauty", "Outdoors", "Mood" }.Contains(__instance.def.defName))
			{
				__result = string.Empty;
			}
		}
	}
}
