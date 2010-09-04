using System.Linq;

namespace Xpand.ExpressApp.Core
{
    public interface ILinqCollectionSource
    {
        IQueryable Query { get; set; }

    }
}