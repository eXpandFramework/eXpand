using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.Core
{
    public static class ModelNodeExtensions
    {
        static readonly MethodInfo _methodInfo;

        static ModelNodeExtensions() {
            _methodInfo = typeof(ModelNode).GetMethod("AddNode", new[] { typeof(string) });
        }

        public static IModelNode AddNode(this IModelNode modelNode,Type type, string id)
        {
            return (IModelNode) _methodInfo.MakeGenericMethod(new[]{type }).Invoke(modelNode, new object[] {id});
        }
    }
}
