using RimWorld;
using Verse;

namespace RimsecSecurity;

internal class HediffComp_EnergyShortage : HediffComp
{
	public override void CompPostTick(ref float severityAdjustment)
	{
		base.CompPostTick(ref severityAdjustment);
		if ((double)base.Pawn.needs.rest.CurLevel > 0.01)
		{
			parent.Severity = 0f;
		}
		if (base.Pawn != null && base.Pawn.Faction != Faction.OfPlayer)
		{
			base.Pawn.Kill(null, parent);
		}
	}
}
