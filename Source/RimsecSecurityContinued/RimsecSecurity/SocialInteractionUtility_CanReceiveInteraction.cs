using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

[HarmonyPatch(typeof(SocialInteractionUtility), "CanReceiveInteraction")]
public class SocialInteractionUtility_CanReceiveInteraction
{
	public static void Postfix(ref bool __result, Pawn pawn, InteractionDef interactionDef = null)
	{
		if (__result && PeacekeeperUtility.IsPeacekeeper(pawn))
		{
			__result = false;
		}
	}
}
