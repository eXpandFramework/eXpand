using System.Linq;

namespace eXpand.ExpressApp.Core
{
    public interface ILinqCollectionSource
    {
        IQueryable Query { get; set; }
//        IList ConvertQueryToCollection(IQueryable sourceQuery);
    }
}