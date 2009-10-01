using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace eXpand.ExpressApp.Core
{
    public class LinqCollectionHelper : ILinqCollectionSource
    {
        public IList ConvertQueryToCollection(IQueryable sourceQuery)
        {
            var list = new List<object>();
            foreach (object item in sourceQuery)
            {
                list.Add(item);
            }
            return list;
        }

        public IQueryable Query { get; set; }
    }
}