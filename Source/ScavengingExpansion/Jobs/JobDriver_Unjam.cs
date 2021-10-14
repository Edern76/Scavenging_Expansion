using System.Collections.Generic;
using ScavengingExpansion.Comps;
using ScavengingExpansion.DefOfs;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ScavengingExpansion.Jobs
{
    public class JobDriver_Unjam : JobDriver
    {
        private TargetIndex indPawn => TargetIndex.A;
        private Pawn wielder => TargetThingA as Pawn;

        private TargetIndex indWeapon => TargetIndex.B;
        private ThingWithComps weapon => TargetThingB as ThingWithComps;

        public override string GetReport()
        {
            string text = JobDefOf.SE_UnjamWeapon.reportString.Replace("TargetB", weapon.def.label);
            return text;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (wielder == null)
            {
                Log.Error("TargetThingA is null, a pawn is required to unjam a weapon");
                yield return null;
            }
            if (weapon == null)
            {
                Log.Error("TargetThingB is null, a weapon is required to be able to unjam it.");
                yield return null;
            }

            CompJammable compJammable = weapon.TryGetComp<CompJammable>();
            if (compJammable == null)
            {
                Log.Error(
                    $"TargetThingB ({weapon.LabelCap}) cannot be jammed or unjammed because it has no Jammable component.");
                yield return null;
            }

            if (!compJammable.Jammed)
            {
                Log.Error($"TargetThingB ({weapon.LabelCap}) is not currently jammed."); //Maybe change it to be a job failure instead ?
                yield return null;
            }
            
            this.FailOnMentalState(indPawn);


            Toil waitToil = Toils_General.Wait(compJammable.Props.UnjamTime.SecondsToTicks())
                .WithProgressBarToilDelay(indPawn);
            yield return waitToil;
            

            //Actually unjam weapon once progress bar complete
            Toil unjamToil = new Toil
            {
                actor = pawn,
                initAction = delegate()
                {
                    compJammable.Unjam(); 
                }
            };

            yield return unjamToil;
            yield break;
        }
    }
}