using System.Collections.Generic;
using System.Linq;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core {
    internal static class AssemblyRouter
    {
        public static List<List<IPersistentClassInfo>> GetLists(List<IPersistentClassInfo> infos)
        {
            var list = new List<List<IPersistentClassInfo>>();
            IEnumerable<string> enumerable = infos.GroupBy(info => info.AssemblyName).Select(grouping => grouping.Key);
            foreach (var s in enumerable)
            {
                string s1 = s;
                var collection = new List<IPersistentClassInfo>();
                collection.AddRange(infos.Where(info => info.AssemblyName == s1));
                list.Add(collection);
            }
            return list;
        }
    }
}