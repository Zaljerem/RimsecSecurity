using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(ThoughtWorker_Precept_AnyBodyPartCovered_Social), "ShouldHaveThought")]
public class ThoughtWorker_Precept_AnyBodyPartCovered_Social_ShouldHaveThought
{
	public static void Postfix(ref ThoughtState __result, Pawn p, Pawn otherPawn)
	{
		if (ModSettings.removeIdeologyImpact && __result.StageIndex != ThoughtState.Inactive.StageIndex && (PeacekeeperUtility.IsPeacekeeper(p) || PeacekeeperUtility.IsPeacekeeper(otherPawn)))
		{
			__result = false;
		}
	}
}
