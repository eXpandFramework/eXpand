using System.Linq.Expressions;

namespace Xpand.Utils.Linq.Dynamic
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}