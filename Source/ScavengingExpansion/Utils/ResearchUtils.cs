using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace ScavengingExpansion.Utils
{
    public static class ResearchUtils
    {
        public static void FinishProjectWithoutPrerequisites(ResearchProjectDef project)
        {
            ResearchManager researchManager = Find.ResearchManager;
            Dictionary<ResearchProjectDef, float> progress = (Dictionary<ResearchProjectDef, float>)typeof(ResearchManager)
                .GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(researchManager); //Access value of private progress field
            progress[project] = project.baseCost;
            typeof(ResearchManager).GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(researchManager, progress); //Set value of private progress field to modified dict
        }
    }
}