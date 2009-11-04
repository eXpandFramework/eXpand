using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using System.Linq;

namespace eXpand.ExpressApp.Core
{
    public static class ViewsNodeWrapper
    {
        public static List<ListViewInfoNodeWrapper> GetListViews(this DevExpress.ExpressApp.NodeWrappers.ViewsNodeWrapper nodeWrapper, Type type){
            return GetListViews(nodeWrapper, XafTypesInfo.CastTypeToTypeInfo(type));
        }

        public static List<ListViewInfoNodeWrapper> GetListViews(this DevExpress.ExpressApp.NodeWrappers.ViewsNodeWrapper nodeWrapper, ITypeInfo typeInfo){
            return GetClassInfoNodeWrapper(nodeWrapper, typeInfo).GetListViews();
        }

        private static ClassInfoNodeWrapper GetClassInfoNodeWrapper(DevExpress.ExpressApp.NodeWrappers.ViewsNodeWrapper nodeWrapper, ITypeInfo typeInfo){
            return new ApplicationNodeWrapper(nodeWrapper.Node.Parent).BOModel.Classes.Where(
                wrapper => wrapper.ClassTypeInfo == typeInfo).Single();
        }

        public static List<DetailViewInfoNodeWrapper> GetDetailViews(this DevExpress.ExpressApp.NodeWrappers.ViewsNodeWrapper nodeWrapper, Type type){
            return GetDetailViews(nodeWrapper, XafTypesInfo.CastTypeToTypeInfo(type));
        }
        public static List<DetailViewInfoNodeWrapper> GetDetailViews(this DevExpress.ExpressApp.NodeWrappers.ViewsNodeWrapper nodeWrapper, ITypeInfo typeInfo)
        {
            return GetClassInfoNodeWrapper(nodeWrapper, typeInfo).GetDetailViews();
        }
    }
}
