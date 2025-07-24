using System.Linq;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Caravan), "get_NightResting")]
public class Caravan_NightResting
{
	public static void Postfix(ref bool __result, Caravan __instance)
	{
		if (__result && __instance.pawns.InnerListForReading.Where((Pawn pawn) => PeacekeeperUtility.IsPeacekeeper(pawn)).Count() == __instance.pawns.Count)
		{
			__result = false;
		}
	}
}
