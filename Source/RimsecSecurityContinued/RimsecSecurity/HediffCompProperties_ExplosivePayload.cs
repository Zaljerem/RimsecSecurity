using RimWorld;
using Verse;

namespace RimsecSecurity
{

    public class HediffCompProperties_ExplosivePayload : HediffCompProperties
    {
        public float explosionRadius = 3.5f;
        public DamageDef damageDef;
        public int damageAmount = -1; // Default to def-defined
        public float armorPenetration = 1.0f;

        public HediffCompProperties_ExplosivePayload()
        {
            this.compClass = typeof(HediffComp_ExplosivePayload);
        }

        public override void ResolveReferences(HediffDef parent)
        {
            base.ResolveReferences(parent);
            if (damageDef == null)
            {
                damageDef = DamageDefOf.Bomb; // Resolve `DefOf` reference here
            }
        }

    }
}

