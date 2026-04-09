using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace RimsecSecurity
{
    [HarmonyPatch(typeof(PawnRelationUtility), nameof(PawnRelationUtility.Notify_PawnsSeenByPlayer))]
    public static class Patch_Notify_PawnsSeenByPlayer
    {
        public static void Prefix(ref IEnumerable<Pawn> seenPawns)
        {
            if (seenPawns != null)
            {
                seenPawns = seenPawns.Where(p =>
                    p != null &&
                    p.relations != null &&
                    !PeacekeeperUtility.IsPeacekeeper(p)
                );
            }
        }
       
    }


    [HarmonyPatch(typeof(PawnRelationUtility), nameof(PawnRelationUtility.Notify_PawnsSeenByPlayer))]
    public static class Patch_Notify_PawnsSeenByPlayer_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var displayClass = AccessTools.TypeByName("RimWorld.PawnRelationUtility+<>c");
            var target = AccessTools.Method(displayClass, "<Notify_PawnsSeenByPlayer>b__2_0");
            var replacement = AccessTools.Method(typeof(Patch_Notify_PawnsSeenByPlayer_Transpiler), nameof(SafeEverSeen));

            var ctor = AccessTools.Constructor(typeof(System.Func<Pawn, bool>), new[] { typeof(object), typeof(IntPtr) });

            foreach (var instr in instructions)
            {
                if (instr.opcode == OpCodes.Ldftn && instr.operand is MethodInfo mi && mi == target)
                {
                    // stack BEFORE:
                    // <>c instance already on stack

                    // replace ONLY the function pointer
                    yield return new CodeInstruction(OpCodes.Ldftn, replacement);
                    continue;
                }

                yield return instr;
            }
        }

        public static bool SafeEverSeen(Pawn p)
        {
            return p?.relations != null && p.relations.everSeenByPlayer;
        }
    }


}
