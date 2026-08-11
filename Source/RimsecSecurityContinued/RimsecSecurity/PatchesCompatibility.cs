using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimsecSecurity;

internal static class PatchesCompatibility
{
	public static Assembly hygieneAssembly;
    public static Assembly hospitalityAssembly;
    public static Assembly orphanageAssembly;
	public static Assembly rimtalkAssembly;

    public static void ExecuteCompatibilityPatches(Harmony harmony)
	{

		Assembly assemblyFromString = PeacekeeperUtility.GetAssemblyFromString("prisonlabor");
		if (assemblyFromString != null)
		{
			Log.Message("[RimsecSecurity] Patching Prison Labor");
			MethodInfo original = AccessTools.Method(assemblyFromString.GetType("PrisonLabor.Core.Needs.Need_Treatment"), "NeedInterval");
			HarmonyMethod prefix = new HarmonyMethod(typeof(PrisonLaberPatches), "Need_Treatment_NeedInterval_Prefix");
			harmony.Patch(original, prefix);
		}

		// RimTalk IsTalkEligible patch
		rimtalkAssembly = PeacekeeperUtility.GetAssemblyFromString("rimtalk");
		if (rimtalkAssembly != null)
		{
			Log.Message("[RimsecSecurity] Patching RimTalk IsTalkEligible method for Peacekeeper check");

			MethodInfo validTalkMethod = AccessTools.Method(rimtalkAssembly.GetType("RimTalk.Util.PawnUtil"), "IsTalkEligible");

			if (validTalkMethod == null)
			{
				Log.Error("Failed to find RimTalk.Util.PawnUtil.IsTalkEligible method!");
				return;
			}

			HarmonyMethod prefixRT = new HarmonyMethod(typeof(RimTalkPatches), nameof(RimTalkPatches.IsTalkEligible_Prefix));
			harmony.Patch(validTalkMethod, prefix: prefixRT);
		}

            // Hospitality ValidGuest patch
            hospitalityAssembly = PeacekeeperUtility.GetAssemblyFromString("hospitality");
		if (hospitalityAssembly != null)
		{
			Log.Message("[RimsecSecurity] Patching Hospitality ValidGuest method for Peacekeeper check");

			// Use AccessTools to access private method
			MethodInfo validGuestMethod = AccessTools.Method(hospitalityAssembly.GetType("Hospitality.Utilities.SpawnGroupUtility"), "ValidGuest");

			if (validGuestMethod == null)
			{
				Log.Error("Failed to find Hospitality.Utilities.SpawnGroupUtility.ValidGuest method!");
				return;
			}

			HarmonyMethod postfix = new HarmonyMethod(typeof(HospitalityPatches), nameof(HospitalityPatches.ValidGuest_Postfix));
			harmony.Patch(validGuestMethod, null, postfix);
		}

		//Assembly assemblyFromString2 = PeacekeeperUtility.GetAssemblyFromString("shipshaveinsides");
		//if (assemblyFromString2 != null)
		//{
		//	Log.Message("[RimsecSecurity] Patching SOS2");
		//	MethodInfo methodInfo = AccessTools.Method(assemblyFromString2.GetType("SaveOurShip2.ShipInteriorMod2"), "hasSpaceSuit");
		//	HarmonyMethod postfix = new HarmonyMethod(typeof(SaveOurShip2Patches), "ShipInteriorMod2_hasSpaceSuit_Postfix");
		//	if (methodInfo == null)
		//	{
		//		methodInfo = AccessTools.Method(assemblyFromString2.GetType("SaveOurShip2.ShipInteriorMod2"), "EVAlevel");
		//		postfix = new HarmonyMethod(typeof(SaveOurShip2Patches), "ShipInteriorMod2_EVAlevel_Postfix");
		//	}
		//	harmony.Patch(methodInfo, null, postfix);
		//}
		Assembly assemblyFromString3 = PeacekeeperUtility.GetAssemblyFromString("guardsforme");
		if (assemblyFromString3 != null)
		{
			Log.Message("[RimsecSecurity] Patching Guards for Me");
			MethodInfo original2 = AccessTools.Method(assemblyFromString3.GetType("aRandomKiwi.GFM.Utils"), "guardNeedFood");
			HarmonyMethod postfix2 = new HarmonyMethod(typeof(GuardsForMePatches), "guardNeedFood_Postfix");
			harmony.Patch(original2, null, postfix2);
			original2 = AccessTools.Method(assemblyFromString3.GetType("aRandomKiwi.GFM.Utils"), "guardNeedJoy");
			postfix2 = new HarmonyMethod(typeof(GuardsForMePatches), "guardNeedJoy_Postfix");
			harmony.Patch(original2, null, postfix2);
			original2 = AccessTools.Method(assemblyFromString3.GetType("aRandomKiwi.GFM.Utils"), "guardNeedMood");
			postfix2 = new HarmonyMethod(typeof(GuardsForMePatches), "guardNeedMood_Postfix");
			harmony.Patch(original2, null, postfix2);
			original2 = AccessTools.Method(assemblyFromString3.GetType("aRandomKiwi.GFM.Utils"), "guardNeedHygiene");
			postfix2 = new HarmonyMethod(typeof(GuardsForMePatches), "guardNeedHygiene_Postfix");
			harmony.Patch(original2, null, postfix2);
			original2 = AccessTools.Method(assemblyFromString3.GetType("aRandomKiwi.GFM.Utils"), "guardNeedBladder");
			postfix2 = new HarmonyMethod(typeof(GuardsForMePatches), "guardNeedBladder_Postfix");
			harmony.Patch(original2, null, postfix2);
		}
		hygieneAssembly = PeacekeeperUtility.GetAssemblyFromString("badhygiene");
		if (hygieneAssembly != null)
		{
			MethodInfo original3 = AccessTools.Method(typeof(Pawn_NeedsTracker), "ShouldHaveNeed");
			HarmonyMethod postfix3 = new HarmonyMethod(typeof(DubsHygienePatches), "Pawn_NeedsTracker_ShouldHaveNeed_Postfix");
			harmony.Patch(original3, null, postfix3);
		}

		orphanageAssembly = PeacekeeperUtility.GetAssemblyFromString("orphanageanddaycare");
		if (orphanageAssembly != null)
		{
			Log.Message("[RimsecSecurity] Patching OrphanageAndDaycare PawnCounts method for Peacekeeper check");

			// Use AccessTools to access private method
			MethodInfo validDaycareMethod = AccessTools.Method(orphanageAssembly.GetType("OrphanageAndDaycare.QuestNode_GetChildToAdopt"), "PawnCounts");

			if (validDaycareMethod == null)
			{
				Log.Error("OrphanageAndDaycare.QuestNode_GetChildToAdopt.PawnCounts method!");
				return;
			}

			HarmonyMethod orphanagePrefix = new HarmonyMethod(typeof(OrphanagePatches), nameof(OrphanagePatches.PawnCounts_Prefix));
            //harmony.Patch(validDaycareMethod, null, orphanagePrefix);
            harmony.Patch(validDaycareMethod, prefix: orphanagePrefix);
        }

	}
}
