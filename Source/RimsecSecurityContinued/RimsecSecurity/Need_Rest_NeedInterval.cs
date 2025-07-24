using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Need_Rest), "NeedInterval")]
public class Need_Rest_NeedInterval
{
	private static float MalnutritionSeverityPerInterval(Pawn pawn)
	{
		return 0.0011333333f * Mathf.Lerp(0.8f, 1.2f, Rand.ValueSeeded(pawn.thingIDNumber ^ 0x26EF7A));
	}

	public static bool Prefix(Need_Rest __instance, Pawn ___pawn, ref int ___ticksAtZero)
	{
		if (!PeacekeeperUtility.IsPeacekeeper(___pawn))
		{
			return true;
		}
		bool value = Traverse.Create(__instance).Property("IsFrozen").GetValue<bool>();
		float num = MalnutritionSeverityPerInterval(___pawn);
		if (!value)
		{
			__instance.CurLevel -= __instance.RestFallPerTick * 150f;
		}
		if (__instance.CurLevel < 0.0001f)
		{
			___ticksAtZero += 150;
		}
		else
		{
			___ticksAtZero = 0;
		}
		if (___ticksAtZero > 1000)
		{
			HealthUtility.AdjustSeverity(___pawn, RSDefOf.RSEnergyShortage, num * ModSettings.energyShortageSeverityMult);
		}
		else
		{
			HealthUtility.AdjustSeverity(___pawn, RSDefOf.RSEnergyShortage, 0f - num);
		}
		return false;
	}
}
