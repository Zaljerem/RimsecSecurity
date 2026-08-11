using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace RimsecSecurity
{

    // No "butchered humanlike" thoughts from dismantling/butchering peacekeepers

    [HarmonyPatch]
    public static class Patch_Corpse_ButcherProducts
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.EnumeratorMoveNext(
                AccessTools.Method(typeof(Corpse), nameof(Corpse.ButcherProducts)));
        }

        public static bool CountsAsHumanButchery(Pawn pawn)
        {
            return pawn.RaceProps.Humanlike &&
                   !PeacekeeperUtility.IsPeacekeeper(pawn);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo racePropsGetter =
                AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.RaceProps));

            MethodInfo humanlikeGetter =
                AccessTools.PropertyGetter(typeof(RaceProperties), nameof(RaceProperties.Humanlike));

            MethodInfo helper =
                AccessTools.Method(typeof(Patch_Corpse_ButcherProducts), nameof(CountsAsHumanButchery));

            bool patched = false;

            var list = new List<CodeInstruction>(instructions);

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Calls(racePropsGetter) &&
                    list[i + 1].Calls(humanlikeGetter))
                {
                    list[i] = new CodeInstruction(OpCodes.Call, helper);

                    list.RemoveAt(i + 1);

                    patched = true;
                    break;
                }
            }

            if (!patched)
            {
                Log.Error("[Rimsec Security] Failed to patch Corpse.ButcherProducts.");
            }

            return list;
        }
    }
}