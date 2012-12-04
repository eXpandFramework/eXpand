using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public delegate XpandUnitOfWork CreateUnitOfWorkHandler();

    public class XpandObjectSpace : XPObjectSpace, IXpandObjectSpace {
        public Func<object, object> GetObjectAction;

        public XpandObjectSpace(ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, CreateUnitOfWorkHandler createUnitOfWorkDelegate)
            : base(typesInfo, xpoTypeInfoSource, createUnitOfWorkDelegate.Invoke) {
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
            var typeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
            return typeInfoSource.TypeIsKnown(type) ? typeInfoSource.GetEntityClassInfo(typeInfo.Type) : null;
        }

        protected override void SetModified(object obj, ObjectChangedEventArgs args) {
            if (args.Object != null && session.Dictionary.QueryClassInfo(args.Object) != null && session.GetClassInfo(args.Object).FindMember(args.PropertyName) is ISupportCancelModification)
                return;
            base.SetModified(obj, args);
        }
    }

    public class NestedXpandObjectSpace : XPNestedObjectSpace {
        public NestedXpandObjectSpace(IObjectSpace parentObjectSpace)
            : base(parentObjectSpace) {
        }
    }
}
