using System.Collections.Generic;
using System.Linq;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Utils;
using UnityEngine;
using Verse;

namespace ScavengingExpansion.Comps
{
    public class CompProperties_AutoLearner : CompProperties
    {

        public CompProperties_AutoLearner()
        {
            this.compClass = typeof(CompAutoLearner);
        }

        public string UIIconPath = "UI/Blueprint/Learn";
        public bool SpawnWithSetProject = false;
        public ResearchProjectSetDef AvailableProjects;
    }

    public class CompAutoLearner : ThingComp
    {
        public CompProperties_AutoLearner Props => (CompProperties_AutoLearner)this.props;

        private List<ResearchProjectDef> ActuallyAvailableProjects =>
            Props.AvailableProjects.projects.Where(proj => !proj.IsFinished).ToList();

        private ResearchProjectDef setProject = null;
        private Texture2D GizmoTex
        {
            get { return ContentFinder<Texture2D>.Get(Props.UIIconPath); }
        }

        private ResearchProjectDef getProjectFromSet()
        {
            List<ResearchProjectDef> projectPool = ActuallyAvailableProjects.Count > 0
                ? ActuallyAvailableProjects
                : Props.AvailableProjects.projects;

            return projectPool[Rand.Range(0, projectPool.Count)];
        }

        private void UseItem()
        {
            ResearchProjectDef toLearn = this.setProject != null ? this.setProject : getProjectFromSet();
            if (!toLearn.IsFinished)
            {
                ResearchUtils.FinishProjectWithoutPrerequisites(toLearn);
                parent.Destroy();
            }
            else
            {
                Log.Warning("Trying to learn already known project");
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (Props.SpawnWithSetProject)
            {
                this.setProject = getProjectFromSet();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                icon = this.GizmoTex,
                defaultLabel = "Use blueprint",
                defaultDesc = "Learn the research project described in the blueprint",
                action = this.UseItem,
                activateSound = SoundDef.Named("Click"),
                disabled = this.setProject?.IsFinished ?? getProjectFromSet() == null
            };
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref setProject, "setProject");
        }


        public override string TransformLabel(string label) =>
            this.setProject != null ? label + $" ({this.setProject.label})" : base.TransformLabel(label);
    }
}    