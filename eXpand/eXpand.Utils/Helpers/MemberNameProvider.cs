using System;
using System.Linq.Expressions;
using System.Reflection;

namespace eXpand.Utils.Helpers
{
    public class MemberNameProvider<T>
    {
        public MemberInfo GetMember<T>(Expression<Func<T, object>> method)
        {
            return ReflectionExtensions.GetExpression(method);
        }

    }
}
