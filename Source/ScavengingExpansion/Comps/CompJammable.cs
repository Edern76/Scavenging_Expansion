using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

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
    }

    public class CompJammable : ThingComp
    {
        public CompProperties_Jammable Props => (CompProperties_Jammable)this.props;
        private Texture2D GizmoTex
        {
            get { Log.Message("Trying to find tex");return ContentFinder<Texture2D>.Get(Props.UIIconPath); }
        }

        private bool _jammed;

        public bool Jammed
        {
            get { return _jammed; }
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

        private void unjam()
        {
            this._jammed = false;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //if (GetWearer != null && GetWearer.IsColonist && Find.Selector.SingleSelectedThing == GetWearer)
            //{
                Log.Message("Getting gizmos");
                if (Jammed)
                {
                    yield return new Command_Action
                    {
                        icon = this.GizmoTex,
                        defaultLabel = "Unjam weapon",
                        defaultDesc = "Unjam a jammed weapon",
                        action = delegate { this.unjam(); },
                        activateSound = SoundDef.Named("Click"),
                    };
                }
                //}
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this._jammed, "jammed", false);
        }
    }    
}