using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimsecSecurity;

[HarmonyPatch(typeof(FloatMenuOptionProvider_RescuePawn), "GetSingleOptionFor")]
public static class FloatMenuOptionProvider_RescuePawn_GetSingleOptionFor
{
    public static void Postfix(ref FloatMenuOption __result, Pawn clickedPawn, FloatMenuContext context)
    {
        if (__result == null)
            return;

        // Check that this is the correct type of option and that the target is a peacekeeper
        if (__result.Priority != MenuOptionPriority.RescueOrCapture || !PeacekeeperUtility.IsPeacekeeper(__result.revalidateClickTarget as Pawn))
            return;

        Pawn victim = __result.revalidateClickTarget as Pawn;
        if (victim == null || !clickedPawn.CanReserve(victim))
            return;

        // Replace the action to instead send them to a charge station
        __result.action = delegate
        {
            Thing emptyChargeStation = PeacekeeperUtility.GetEmptyChargeStation(victim);
            if (emptyChargeStation == null)
            {
                string text = (!victim.RaceProps.Animal) ? "NoNonPrisonerBed".Translate() : "NoAnimalBed".Translate();
                Messages.Message("CannotRescue".Translate() + ": " + text, victim, MessageTypeDefOf.RejectInput, false);
            }
            else
            {
                Job job = JobMaker.MakeJob(RSDefOf.RSRescueToChargeStation, victim, emptyChargeStation);
                job.count = 1;
                clickedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.Rescuing, KnowledgeAmount.Total);
            }
        };
    }
}
