using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_Overclockable : CompProperties
    {
        public CompProperties_Overclockable()
        {
            this.compClass = typeof(CompOverclockable);
        }

        public float AdditionalJamChance = 0.04f;
        public string UIIconPath = "UI/Overclock/Overclock";
        public int MaxOverclockTicks = 600;
        public float WeaponCooldownOffsetFactor = 1.3f;
        public float WeaponAccuracyOffsetFactor = 0.8f;
        public float AimingDelayOffsetFactor = 0.8f;
        public float RangeOffsetFactor = 1.15f;
        public float AdditionalEMPDamagePercent = 0.2f;
        public float HandsBurningChance = 0.5f;
        public float HandsDamage = 5f;

    }

    public class CompOverclockable : ThingComp
    {
        public CompProperties_Overclockable Props => (CompProperties_Overclockable)this.props;

        private float previousRange = -1f;
        
        private bool _overclocked = false;
        public bool Overclocked => _overclocked;

        private bool _onCooldown = false;
        public bool OnCooldown => _onCooldown;

        private int _overclockTicks = 0;
        public int OverclockTicks => _overclockTicks;
        
        private Texture2D GizmoTex
        {
            get { return ContentFinder<Texture2D>.Get(Props.UIIconPath); }
        }
        
        public virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Overclocked)
            {
                _overclockTicks++;
                if (OverclockTicks >= Props.MaxOverclockTicks)
                {
                    UndoOverclock();
                    _onCooldown = true;
                }
            }
            else if (OnCooldown)
            {
                _overclockTicks--;
                if (OverclockTicks <= 0)
                {
                    _overclockTicks = 0;
                    _onCooldown = false;
                }
            }
        }

        public void DoOverclock()
        {
            if (this.parent.TryGetComp<CompEquippable>() is CompEquippable compEquippable && compEquippable.PrimaryVerb is Verb verb)
            {
                previousRange = verb.verbProps.range;
                verb.verbProps.range = verb.verbProps.range * this.Props.RangeOffsetFactor;
                _overclocked = true;
            }
        }

        public void UndoOverclock()
        {
            if (this.parent.TryGetComp<CompEquippable>() is CompEquippable compEquippable && compEquippable.PrimaryVerb is Verb verb)
            {
                verb.verbProps.range = previousRange;
                _overclocked = false;
            }
        }

        private void ToggleOverclock()
        {
            if (Overclocked)
            {
                UndoOverclock();
            }
            else
            {
                DoOverclock();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GetWearer != null && GetWearer.IsColonist && Find.Selector.SingleSelectedThing == GetWearer)
            {
                yield return new Command_Toggle()
                {
                    icon = this.GizmoTex,
                    defaultLabel = "Overclock weapon",
                    defaultDesc =
                        "Toggle whether or not to overclock the weapon.",
                    toggleAction = ToggleOverclock,
                    isActive = (() => Overclocked),
                    disabled = OnCooldown,
                    activateSound = SoundDef.Named("Click"),
                };
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);;
            if (previousRange < 0f)
            {
                Verb verb = this.parent.TryGetComp<CompEquippable>().PrimaryVerb;
                float verbRange = verb.verbProps.range;
                previousRange = verbRange;
                if (Overclocked)
                {
                    this.parent.TryGetComp<CompEquippable>().PrimaryVerb.verbProps.range =
                        previousRange * this.Props.RangeOffsetFactor;
                }
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref _overclocked, "overclocked", false);
            Scribe_Values.Look(ref _onCooldown, "onCooldown", false);
            Scribe_Values.Look(ref _overclockTicks, "overclockTicks", 0);
        }
    }
}