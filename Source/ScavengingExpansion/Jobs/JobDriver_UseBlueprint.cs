using System;
using System.Collections.Generic;
using ScavengingExpansion.Comps;
using Verse;
using Verse.AI;

namespace ScavengingExpansion.Jobs
{
    public class JobDriver_UseBlueprint : JobDriver
    {
        private const TargetIndex BlueprintInd = TargetIndex.A;
        private const int Duration = 600;

        protected Thing Blueprint => this.job.GetTarget(TargetIndex.A).Thing;
        protected CompAutoLearner comp => this.Blueprint?.TryGetComp<CompAutoLearner>();

        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            this.pawn.Reserve((LocalTargetInfo)this.Blueprint, this.job, errorOnFailed : errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
                .FailOnDespawnedNullOrForbidden<Toil>(TargetIndex.A)
                .FailOnSomeonePhysicallyInteracting<Toil>(TargetIndex.A);
            yield return Toils_General.Wait(Duration)
                .FailOnDestroyedNullOrForbidden<Toil>(TargetIndex.A)
                .WithEffect(comp?.Props.StudyingEffect, TargetIndex.A)
                .WithProgressBarToilDelay(TargetIndex.A);
            yield return new Toil()
            {
                initAction = delegate()
                {
                    comp?.UseItem();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}