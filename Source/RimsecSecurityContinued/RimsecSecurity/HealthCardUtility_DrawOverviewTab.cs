using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
public class HealthCardUtility_DrawOverviewTab
{
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		MethodInfo getLabelForInfo = AccessTools.Method(typeof(PawnCapacityDef), "GetLabelFor", new Type[2]
		{
			typeof(bool),
			typeof(bool)
		});
		int num = list.FindIndex((CodeInstruction code) => code.operand == getLabelForInfo);
		if (num == -1)
		{
			Log.Warning("Could not find GetLabelFor code instruction; skipping changes");
			return instructions;
		}
		list[num].operand = AccessTools.Method(typeof(PawnCapacityDef), "GetLabelFor", new Type[1] { typeof(Pawn) });
		return list;
	}
}
