using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(ThoughtWorker_Precept_IdeoDiversity_Uniform), "ShouldHaveThought")]
public class ThoughtWorker_Precept_IdeoDiversity_Uniform_ShouldHaveThought
{
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		if (!ModSettings.removeIdeologyImpact)
		{
			return list;
		}
		int num = list.FindIndex((CodeInstruction code) => code.operand != null && code.operand.ToString().Contains("IsQuestLodger"));
		if (num == -1)
		{
			Log.Warning("Could not find IsQuestLodger code instruction; skipping changes");
			return instructions;
		}
		list.Insert(num + 2, new CodeInstruction(OpCodes.Ldloc_0));
		list.Insert(num + 3, new CodeInstruction(OpCodes.Ldloc_2));
		list.Insert(num + 4, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<Pawn>), "get_Item", new Type[1] { typeof(int) })));
		list.Insert(num + 5, new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PeacekeeperUtility), "IsPeacekeeper", new Type[1] { typeof(Pawn) })));
		list.Insert(num + 6, new CodeInstruction(OpCodes.Brtrue_S, list[num + 1].operand));
		return list;
	}
}
