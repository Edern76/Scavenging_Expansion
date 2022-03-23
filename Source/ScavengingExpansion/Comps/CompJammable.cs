using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using JobDefOf = ScavengingExpansion.DefOfs.JobDefOf;
using SoundDefOf = ScavengingExpansion.DefOfs.SoundDefOf;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_Jammable : CompProperties
    {
        public CompProperties_Jammable()
        {
            this.compClass = typeof(CompJammable);
        }

        public float JamChancePerShot = 0.05f;
        public string UIIconPath = "UI/Unjam/Unjam";
        public float UnjamTime = 2f;
        public SoundDef JamSound = null;
        public float ExplosionOnJamChance = 0f;
        public float ExplosiveRadius = 1.9f;
        public DamageDef ExplosiveDamageType;
        public int DamageAmountBase = -1;
        public float ArmorPenetrationBase = -1f;

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);
            if (JamSound == null)
            {
                JamSound = SoundDefOf.SE_JammedSound;
            }

            if (ExplosiveDamageType == null)
            {
                ExplosiveDamageType = DamageDefOf.Bomb;
            }    
        }
    }

    public class CompJammable : ThingComp
    {
        public CompProperties_Jammable Props => (CompProperties_Jammable)this.props;
        private Texture2D GizmoTex
        {
            get { return ContentFinder<Texture2D>.Get(Props.UIIconPath); }
        }

        private bool _jammed;
        private bool _autoUnjam = true;
        private int _shotsSinceLastBurst = 0;

        public bool Jammed
        {
            get { return _jammed; }
        }

        public bool AutoUnjam
        {
            get { return _autoUnjam; }
            set { _autoUnjam = value; }
        }

        public int ShotsSinceLastBurst
        {
            get { return _shotsSinceLastBurst; }
            set { _shotsSinceLastBurst = value; }
        }
        
        protected virtual Pawn GetWearer
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
        

        public void setJammed()
        {
            if (this.Props.JamSound != null)
            {
                this.Props.JamSound.PlayOneShot(SoundInfo.InMap((TargetInfo)(Thing)this.GetWearer));
            }    
            this._jammed = true;
        }

        public void Unjam()
        {
            this._jammed = false;
        }

        public Job TryMakeJob()
        {
            return GetWearer == null ? null : JobMaker.MakeJob(JobDefOf.SE_UnjamWeapon, GetWearer, parent);
        }

        public void DoExplode()
        {
            if (this.Props.ExplosionOnJamChance == 0f)
            {
                Log.Error("Trying to explode a weapon with 0 explosion chance");
                return;
            }    
            
            GenExplosion.DoExplosion(GetWearer.PositionHeld, GetWearer.Map, Props.ExplosiveRadius, Props.ExplosiveDamageType, GetWearer, Props.DamageAmountBase, Props.ArmorPenetrationBase);
            this.parent.Destroy();
        }

        public float GetJamChance()
        {
            CompOverclockable compOverclockable = parent.TryGetComp<CompOverclockable>();
            if (compOverclockable != null)
            {
                return Mathf.Min(Props.JamChancePerShot + compOverclockable.Props.AdditionalJamChance, 1f);
            }
            else
            {
                return Props.JamChancePerShot;
            }
        }

        private void tryStartUnjam()
        {
            Job unjamJob = TryMakeJob();
            if (unjamJob != null)
            {
                GetWearer.jobs.StopAll();
                GetWearer.jobs.TryTakeOrderedJob(unjamJob);
            }
            else
            {
                Log.Warning("UnjamJob is null");
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GetWearer != null && GetWearer.IsColonist && Find.Selector.SingleSelectedThing == GetWearer)
            {
                if (Jammed)
                    {
                        yield return new Command_Action
                        {
                            icon = this.GizmoTex,
                            defaultLabel = "Unjam weapon",
                            defaultDesc = "Unjam a jammed weapon",
                            action = this.tryStartUnjam,
                            activateSound = SoundDef.Named("Click"),
                        };
                    }

                yield return new Command_Toggle()
                {
                    icon = this.GizmoTex,
                    defaultLabel = "Auto unjamming",
                    defaultDesc =
                        "Toggle whether or not the wielder of this weapon should automatically try to unjam it.",
                    toggleAction = delegate { AutoUnjam = !AutoUnjam; },
                    isActive = (() => AutoUnjam),
                    activateSound = SoundDef.Named("Click"),
                };
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this._jammed, "jammed", false);
            Scribe_Values.Look(ref this._autoUnjam, "autoUnjam", true);
            Scribe_Values.Look(ref _shotsSinceLastBurst, "shotsSinceLastBurst", 0);
        }
    }    
}