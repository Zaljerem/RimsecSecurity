using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RimsecSecurity;

public class Main : Mod
{
	public Main(ModContentPack content)
		: base(content)
	{
		GetSettings<ModSettings>();

        Harmony harmony = new Harmony("Shakesthespeare.RimsecSecurity");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Patch_TryAppendRelationsWithColonistsInfo.ApplyPatch(harmony);
        Patch_TryAppendRelationsWithColonistsInfoTwo.ApplyPatch(harmony);
        PatchesCompatibility.ExecuteCompatibilityPatches(harmony);
        Patch_UsefulMarksMarkerConditionRelations.Apply(harmony);

        if (ModSettings.hidePeacekeepersFromColonistBar)
        {
            MethodInfo original = AccessTools.Method(typeof(ColonistBar), "CheckRecacheEntries");
            HarmonyMethod postfix = new HarmonyMethod(typeof(TargettedPatches), "ColonistBar_CheckRecacheEntries_Postfix");
            harmony.Patch(original, null, postfix);
        }
        if (!ModSettings.countPeacekeepersTowardsPopulation)
        {
            MethodInfo original2 = AccessTools.PropertyGetter(typeof(StorytellerUtilityPopulation), "AdjustedPopulation");
            HarmonyMethod postfix2 = new HarmonyMethod(typeof(TargettedPatches), "StorytellerUtilityPopulation_AdjustedPopulation_get_Postfix");
            harmony.Patch(original2, null, postfix2);
        }

        List<Patches> patchInfo = harmony.GetPatchedMethods().Select(Harmony.GetPatchInfo).ToList();
        int prefixCount = patchInfo.SelectMany(p => p.Prefixes).Count(predicate: p => p.owner == harmony.Id);
        int postfixCount = patchInfo.SelectMany(p => p.Postfixes).Count(predicate: p => p.owner == harmony.Id);
        int transpilerCount = patchInfo.SelectMany(p => p.Transpilers).Count(predicate: p => p.owner == harmony.Id);

         Log.Message($"[RimsecSecurity] Applied {prefixCount + postfixCount + transpilerCount} patches ({prefixCount} pre, {postfixCount} post, {transpilerCount} trans)");
    }

	public override void DoSettingsWindowContents(Rect inRect)
	{
		base.DoSettingsWindowContents(inRect);
		GetSettings<ModSettings>().DoWindowContents(inRect);
	}

	public override string SettingsCategory()
	{
		return "RS_ModName".Translate();
	}

  
   
}
