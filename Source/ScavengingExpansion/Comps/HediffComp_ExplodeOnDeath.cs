using RimWorld;
using Verse;

namespace ScavengingExpansion.Comps
{
    public class HediffCompProperties_ExplodeOnDeath : HediffCompProperties
    {
        
        public float explosiveRadius = 1.9f;
        public DamageDef explosiveDamageType;
        public int damageAmountBase = -1;
        public float armorPenetrationBase = -1f;
        
        
        public HediffCompProperties_ExplodeOnDeath()
        {
            this.compClass = typeof(HediffComp_ExplodeOnDeath);
        }

        public override void ResolveReferences(HediffDef parent)
        {
            base.ResolveReferences(parent);
            if (this.explosiveDamageType != null)
                return;
            this.explosiveDamageType = DamageDefOf.Bomb;
        }
    }
    
    public class HediffComp_ExplodeOnDeath : HediffComp
    {
        public HediffCompProperties_ExplodeOnDeath Props => (HediffCompProperties_ExplodeOnDeath)this.props;
        private Pawn AffectedPawn => this.parent.pawn;

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            Map map = AffectedPawn.Map;
            if (map == null)
            {
                map = AffectedPawn.Corpse?.Map;
            }    
            GenExplosion.DoExplosion(AffectedPawn.PositionHeld, map, Props.explosiveRadius,
                Props.explosiveDamageType, AffectedPawn, Props.damageAmountBase, Props.armorPenetrationBase);
        }
    }
}