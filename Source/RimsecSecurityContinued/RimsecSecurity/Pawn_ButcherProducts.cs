using HarmonyLib;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(Pawn), "ButcherProducts")]
public class Pawn_ButcherProducts
{
	public static void Postfix(Pawn __instance, Pawn butcher, float efficiency)
	{
		ModSettings.butcheredPeacekeeper = PeacekeeperUtility.IsPeacekeeper(__instance);
	}
}
