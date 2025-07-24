using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(SocialInteractionUtility), "CanInitiateRandomInteraction")]
public class SocialInteractionUtility_CanInitiateRandomInteraction
{
	public static void Postfix(ref bool __result, Pawn p)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(p))
		{
			__result = false;
		}
	}
}
