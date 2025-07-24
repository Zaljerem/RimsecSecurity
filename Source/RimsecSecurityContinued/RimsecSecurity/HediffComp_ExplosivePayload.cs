using Verse;

namespace RimsecSecurity
{
    public class HediffComp_ExplosivePayload : HediffComp
    {
        public HediffCompProperties_ExplosivePayload Props => (HediffCompProperties_ExplosivePayload)props;

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();

            // Ensure the HediffComp is attached to a pawn
            if (Pawn == null)
            {
                Log.Warning("HediffComp_ExplosivePayload triggered on a null Pawn.");
                return;
            }

            TriggerExplosion();

            // Remove the Hediff
            Hediff hediff = Pawn.health.hediffSet.GetFirstHediffOfDef(this.parent.def);
            if (hediff != null)
            {
                Pawn.health.RemoveHediff(hediff);
            }

        }

        public void TriggerExplosion()
        {
            if (Pawn == null || Pawn.Map == null)
            {
                return;
            }

            GenExplosion.DoExplosion(
                center: Pawn.Position,
                map: Pawn.Map,
                radius: Props.explosionRadius,
                damType: Props.damageDef,
                instigator: Pawn,
                damAmount: Props.damageAmount,
                armorPenetration: Props.armorPenetration
            );
        }
    }
}
