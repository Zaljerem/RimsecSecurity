using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(IncidentWorker_DiseaseHuman), "PotentialVictimCandidates")]
public class IncidentWorker_DiseaseHuman_PotentialVictimCandidates
{
	public static void Postfix(ref IEnumerable<Pawn> __result, IIncidentTarget target)
	{
		List<Pawn> list = __result.ToList();
		list.RemoveAll((Pawn pawn) => PeacekeeperUtility.IsPeacekeeper(pawn));
		__result = list;
	}
}
