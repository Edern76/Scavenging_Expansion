using System.Collections;
using System.Collections.Generic;
using ScavengingExpansion.Buildings;
using Verse;
using Verse.AI;

namespace ScavengingExpansion.Jobs
{
    public class JobDriver_Excavate : JobDriver
    {
        private TargetIndex indExcavator => TargetIndex.A;
        private Building_Excavator excavator => TargetThingA as Building_Excavator;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(excavator, this.job, 1);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(indExcavator);
            yield return Toils_Goto.GotoThing(indExcavator, PathEndMode.Touch);
            yield return Toils_General.Wait(excavator.ExcavationTime.SecondsToTicks())
                .WithProgressBarToilDelay(indExcavator);
            yield return new Toil
            {
                initAction = delegate
                {
                    foreach (Thing product in excavator.MakeRessources(this.pawn))
                    {
                        GenPlace.TryPlaceThing(product, this.pawn.Position, this.pawn.Map, ThingPlaceMode.Near);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }
    }
}