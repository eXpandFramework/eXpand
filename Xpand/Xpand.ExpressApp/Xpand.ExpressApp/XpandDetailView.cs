using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.MemberLevelSecurity;

namespace Xpand.ExpressApp {
    public class XpandDetailView : DetailView {


        public XpandDetailView(IModelDetailView info, IObjectSpace objectSpace, object obj, XafApplication application, bool isRoot)
            : base(info, objectSpace, obj, application, isRoot) {
        }

        public XpandDetailView(IObjectSpace objectSpace, object obj, XafApplication application, bool isRoot)
            : base(objectSpace, obj, application, isRoot) {
        }

        protected override ViewItem CreateItem(IModelViewItem info) {
            if (Application == null)
                return null;
            if (!IsMemberLelvelSecurityInstalled())
                return base.CreateItem(info);
            Type objType = ObjectTypeInfo != null ? ObjectTypeInfo.Type : null;
            return Application.EditorFactory.CreateDetailViewEditor(false, info, objType, Application, ObjectSpace);

        }

        bool IsMemberLelvelSecurityInstalled() {
            return Application.Modules.OfType<IMemberLevelSecurityModule>().FirstOrDefault()!=null;
        }
    }

}