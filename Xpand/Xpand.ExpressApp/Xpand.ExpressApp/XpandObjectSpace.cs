using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.ExpressApp {

    public class XpandObjectSpace : ObjectSpace, IXpandObjectSpace {
        public Func<object, object> GetObjectAction;
        public XpandObjectSpace(UnitOfWork unitOfWork, ITypesInfo typesInfo)
            : base(unitOfWork, typesInfo) {
        }
        public override object GetObject(object objectFromDifferentObjectSpace) {
            return GetObjectAction != null ? GetObjectAction.Invoke(objectFromDifferentObjectSpace) : base.GetObject(objectFromDifferentObjectSpace);
        }
        
        public object FindObject(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria, bool inTransaction,
                                 bool selectDeleted) {
            CheckIsDisposed();
            XPClassInfo classInfo = FindXPClassInfo(objectType);
            object result = inTransaction ? session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, classInfo, criteria, selectDeleted)
                                : session.FindObject(classInfo, criteria, selectDeleted);
            return result;
        }
        public override object CreateObject(Type type) {
            try {
                return base.CreateObject(type);
            } catch (ObjectCreatingException) {
                if (!(type.IsInterface)) {
                    XPClassInfo classInfo = FindXPClassInfo(type);
                    var newObject = classInfo.CreateNewObject(session);
                    SetModified(newObject);
                    return newObject;
                }
                throw;
            }
        }

        private XPClassInfo FindXPClassInfo(Type type) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            return XafTypesInfo.XpoTypeInfoSource.TypeIsKnown(type) ? XafTypesInfo.XpoTypeInfoSource.GetEntityClassInfo(typeInfo.Type) : null;
        }

        protected override void SetModified(object obj, ObjectChangedEventArgs args) {
            if (args.Object != null && session.Dictionary.QueryClassInfo(args.Object) != null && session.GetClassInfo(args.Object).FindMember(args.PropertyName) is ISupportCancelModification)
                return;
            base.SetModified(obj, args);
        }
        public new Action<ResolveSessionEventArgs> AsyncServerModeSourceDismissSession {
            get { return base.AsyncServerModeSourceDismissSession; }
            set { base.AsyncServerModeSourceDismissSession = value; }
        }
        public new Action<ResolveSessionEventArgs> AsyncServerModeSourceResolveSession {
            get { return base.AsyncServerModeSourceResolveSession; }
            set { base.AsyncServerModeSourceResolveSession = value; }
        }

        protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer) {
            return new XpandUnitOfWork(dataLayer);
        }
    }

    public class NestedXpandObjectSpace : NestedObjectSpace {
        public NestedXpandObjectSpace(IObjectSpace parentObjectSpace) : base(parentObjectSpace) {
        }
    }
}
