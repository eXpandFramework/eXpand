using System.Collections.Generic;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public static class XPDictionaryExtensions
    {
        public static void AddClasses(this XPDictionary xpDictionary, List<PersistentClassInfo> infos)
        {
            PersistentClassInfo.FillDictionary(xpDictionary, infos);
            
        }    
    }
}