using System.Linq.Expressions;

namespace eXpand.Utils.Linq.Dynamic
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}