using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using ScavengingExpansion.Defs;
using ScavengingExpansion.Utils;
using UnityEngine;
using Verse;
using Verse.AI;
using JobDefOf = ScavengingExpansion.DefOfs.JobDefOf;

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
        public EffecterDef StudyingEffect;
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

        public void UseItem()
        {
            ResearchProjectDef toLearn = this.setProject != null ? this.setProject : getProjectFromSet();
            if (!toLearn.IsFinished)
            {
                ResearchUtils.FinishProjectWithoutPrerequisites(toLearn);
                Find.LetterStack.ReceiveLetter((TaggedString)$"{toLearn.label.CapitalizeFirst()} discovered", (TaggedString)$"By studying {this.parent.def.label}, your colonists have figured out the intricacies of {toLearn.label}.", LetterDefOf.PositiveEvent);
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
        
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            JobFailReason.Clear();
            if (selPawn.WorkTypeIsDisabled(WorkTypeDefOf.Research) || selPawn.WorkTagIsDisabled(WorkTags.Intellectual))
                JobFailReason.Is((string) "WillNever".Translate((NamedArgument) "Research".TranslateSimple().UncapitalizeFirst()));
            else if (!selPawn.CanReach((LocalTargetInfo) (Thing) this.parent, PathEndMode.ClosestTouch, Danger.Some))
                JobFailReason.Is((string) "CannotReach".Translate());
            else if (!selPawn.CanReserve((LocalTargetInfo) (Thing) this.parent))
            {
                Pawn pawn = selPawn.Map.reservationManager.FirstRespectedReserver((LocalTargetInfo) (Thing) this.parent, selPawn) ?? selPawn.Map.physicalInteractionReservationManager.FirstReserverOf((LocalTargetInfo) (Thing) selPawn);
                if (pawn != null)
                    JobFailReason.Is((string) "ReservedBy".Translate((NamedArgument) pawn.LabelShort, (NamedArgument) (Thing) pawn));
                else
                    JobFailReason.Is((string) "Reserved".Translate());
            }

            Job job = JobMaker.MakeJob(JobDefOf.SE_StudyBlueprint);
            job.targetA = (LocalTargetInfo)(Thing)this.parent;
            if (this.setProject?.IsFinished ?? getProjectFromSet() == null)
            {
                yield return new FloatMenuOption("You won't learn anything new from this", (Action)null);
                JobFailReason.Clear();
            }
            else
            {
                if (JobFailReason.HaveReason)
                {
                    yield return new FloatMenuOption(
                        (string)("CannotGenericWorkCustom".Translate(
                                     (NamedArgument)"ApplyTechprint".Translate((NamedArgument)this.parent.Label)) +
                                 ": " +
                                 JobFailReason.Reason.CapitalizeFirst()), (Action)null);
                    JobFailReason.Clear();
                }
                else
                {
                    yield return new FloatMenuOption($"Study {this.parent.def.label}",
                        delegate() { selPawn.jobs.TryTakeOrderedJob(job); });
                }
            }
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref setProject, "setProject");
        }


        public override string TransformLabel(string label) =>
            this.setProject != null ? label + $" ({this.setProject.label})" : base.TransformLabel(label);
    }
}    