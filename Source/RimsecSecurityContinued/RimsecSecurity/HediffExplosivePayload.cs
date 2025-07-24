using Verse;

namespace RimsecSecurity
{
    public class HediffExplosivePayload : HediffWithComps
    {
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (pawn.Dead)
            {
                // Trigger explosion logic if the pawn is already dead when this Hediff is removed
                TryExplode();
            }
        }

        private void TryExplode()
        {
            var comp = this.TryGetComp<HediffComp_ExplosivePayload>();
            comp?.TriggerExplosion();
        }
    }

}

