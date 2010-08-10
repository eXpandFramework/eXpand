using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using FeatureCenter.Base;
using System.Linq;

namespace FeatureCenter.Module.WorldCreator.DynamicAssemblyMasterDetail
{
    public class WorldCreatorUpdater:eXpand.ExpressApp.WorldCreator.WorldCreatorUpdater
    {
        public const string MasterDetailDynamicAssembly = "MasterDetailDynamicAssembly";
        private const string DMDOrder = "DMDOrder";

        private const string DMDOrderLine = "DMDOrderLine";

        public const string DMDCustomer = "DMDCustomer";

        public WorldCreatorUpdater(Session session) : base(session) {
        }


        public override void Update() {
            var unitOfWork = new UnitOfWork(Session.DataLayer);
            var objectSpace = new ObjectSpace(unitOfWork, XafTypesInfo.Instance);   
            
            if (objectSpace.Session.FindObject<PersistentAssemblyInfo>(CriteriaOperator.Parse("Name=?", MasterDetailDynamicAssembly)) == null){
                IClassInfoHandler classInfoHandler = PersistentAssemblyBuilder.BuildAssembly(objectSpace, MasterDetailDynamicAssembly).CreateClasses(GetClassNames());
                classInfoHandler.CreateTemplateInfos(GetTemplateInfos);
                classInfoHandler.SetInheritance(info => GetInheritance(info));
                classInfoHandler.CreateReferenceMembers(classInfo => GetReferenceMembers(classInfo), true);
                objectSpace.CommitChanges();
            }

        }

        List<ITemplateInfo> GetTemplateInfos(IPersistentClassInfo info) {
            if (info.Name==DMDOrder) {
                return GetOrderTemplateInfo(info);
            }
            if (info.Name==DMDOrderLine) {
                return GetOrderLineTemplateInfo(info);
            }
            return new List<ITemplateInfo>();
            
        }

        List<ITemplateInfo> GetOrderLineTemplateInfo(IPersistentClassInfo info) {
            string code = @"protected override void SetOrder(" + typeof(IOrder).FullName + @" order){
                               " + DMDOrder + @" = (" + DMDOrder + @")order;
                        }
                        protected override "+typeof(IOrder).FullName + @" GetOrder() {
                            return "+DMDOrder + @";
                        }";
            return new List<ITemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }

        List<ITemplateInfo> GetOrderTemplateInfo(IPersistentClassInfo info) {
            string code = @"protected override void SetCustomer("+typeof(ICustomer).FullName + @" customer){
                               "+DMDCustomer + @" = ("+DMDCustomer + @")customer;
                        }
                        protected override " + typeof(ICustomer).FullName + @" GetCustomer() {
                            return " + DMDCustomer + @";
                        }";
            return new List<ITemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }

        IEnumerable<IPersistentClassInfo> GetReferenceMembers(IPersistentClassInfo classInfo) {
            if (classInfo.Name==DMDOrder) {
                return new List<IPersistentClassInfo>{classInfo.PersistentAssemblyInfo.PersistentClassInfos.Where(info => info.Name==DMDCustomer).Single()};
            }
            if (classInfo.Name==DMDOrderLine) {
                return new List<IPersistentClassInfo>{classInfo.PersistentAssemblyInfo.PersistentClassInfos.Where(info => info.Name==DMDOrder).Single()};
            }
            return new List<IPersistentClassInfo>();
        }


        IEnumerable<string> GetClassNames() {
            return new[] {DMDCustomer,DMDOrder,DMDOrderLine};
        }

        Type GetInheritance(IPersistentClassInfo info) {
            switch (info.Name) {
                case DMDCustomer:
                    return typeof(CustomerBase);
                case DMDOrder:
                    return typeof(OrderBase);
                default:
                    return typeof(OrderLineBase);
            }
        }
    }
}
