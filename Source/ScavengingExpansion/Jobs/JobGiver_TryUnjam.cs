using ScavengingExpansion.Comps;
using Verse;
using Verse.AI;

namespace ScavengingExpansion.Jobs
{
    public class JobGiver_TryUnjam : ThinkNode_JobGiver
    {
        private const float PRIORITY = 10f;

        private CompJammable getCompFromPawn(Pawn pawn) => pawn.equipment?.Primary?.TryGetComp<CompJammable>();

        private bool canUnjam(Pawn pawn)
        {
            CompJammable comp = getCompFromPawn(pawn);
            if (comp == null)
            {
                return false;
            }

            if (pawn.IsColonist && comp.AutoUnjam == false)
            {
                return false;
            }    
            return comp.Jammed;
        }
        
        public override float GetPriority(Pawn pawn)
        {
            return canUnjam(pawn) ? PRIORITY : 0f;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (canUnjam(pawn))
            {
                return getCompFromPawn(pawn)?.TryMakeJob();
            }

            return null;
        }
    }
}