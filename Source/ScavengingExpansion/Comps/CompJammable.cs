using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using JobDefOf = ScavengingExpansion.DefOfs.JobDefOf;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_Jammable : CompProperties
    {
        public CompProperties_Jammable()
        {
            this.compClass = typeof(CompJammable);
        }

        public float JamChancePerShot = 0.05f;
        public string UIIconPath = string.Empty;
        public float UnjamTime = 2f;
    }

    public class CompJammable : ThingComp
    {
        public CompProperties_Jammable Props => (CompProperties_Jammable)this.props;
        private Texture2D GizmoTex
        {
            get { return ContentFinder<Texture2D>.Get(Props.UIIconPath); }
        }

        private bool _jammed;
        private bool _autoUnjam;

        public bool Jammed
        {
            get { return _jammed; }
        }

        public bool AutoUnjam
        {
            get { return _autoUnjam; }
            set { _autoUnjam = value; }
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
        }
    }    
}