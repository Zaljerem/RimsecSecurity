using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(SocialInteractionUtility), "CanReceiveRandomInteraction")]
public class SocialInteractionUtility_CanReceiveRandomInteraction
{
	public static void Postfix(ref bool __result, Pawn p)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(p))
		{
			__result = false;
		}
	}
}
