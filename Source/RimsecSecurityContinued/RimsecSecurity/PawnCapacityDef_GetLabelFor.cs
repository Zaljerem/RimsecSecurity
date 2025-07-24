using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(PawnCapacityDef), "GetLabelFor", new Type[] { typeof(Pawn) })]
public class PawnCapacityDef_GetLabelFor
{
	public static void Postfix(ref string __result, PawnCapacityDef __instance, Pawn pawn)
	{
		if (PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			if (__instance == PawnCapacityDefOf.BloodFiltration)
			{
				__result = "RSCoolantFiltration".Translate();
			}
			if (__instance == PawnCapacityDefOf.BloodPumping)
			{
				__result = "RSCoolantCirculation".Translate();
			}
		}
	}
}
