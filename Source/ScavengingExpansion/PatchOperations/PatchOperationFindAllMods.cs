using Verse;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ScavengingExpansion.PatchOperations
{
    public class PatchOperationFindAllMods : PatchOperation
    {
        private List<string> mods;
        private PatchOperation match;
        private PatchOperation nomatch;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool foundAllMods = mods.All(mod => ModLister.HasActiveModWithName(mod));
            if (foundAllMods)
            {
                return this.match?.Apply(xml) ?? true;
            }
            else
            {
                return this.nomatch?.Apply(xml) ?? true;
            }
        }
    }
}