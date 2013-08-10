using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class DynamicAssemblyBuilder {
        readonly Session _session;

        public DynamicAssemblyBuilder(Session session) {
            _session = session;
        }

        List<ITemplateInfo> GetTemplateInfos(IPersistentClassInfo info, string customer, string order) {
            if (info.Name.ToLower().EndsWith("order")) {
                return GetOrderTemplateInfo(info, customer);
            }
            if (info.Name.ToLower().EndsWith("orderline")) {
                return GetOrderLineTemplateInfo(info, order);
            }
            return new List<ITemplateInfo>();

        }

        List<ITemplateInfo> GetOrderLineTemplateInfo(IPersistentClassInfo info, string order) {
            string code = @"protected override void SetOrder(" + typeof(IOrder).FullName + @" order){
                               " + order + @" = (" + order + @")order;
                        }
                        protected override " + typeof(IOrder).FullName + @" GetOrder() {
                            return " + order + @";
                        }";
            return new List<ITemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }

        List<ITemplateInfo> GetOrderTemplateInfo(IPersistentClassInfo info, string customer) {
            string code = @"protected override void SetCustomer(" + typeof(ICustomer).FullName + @" customer){
                               " + customer + @" = (" + customer + @")customer;
                        }
                        protected override " + typeof(ICustomer).FullName + @" GetCustomer() {
                            return " + customer + @";
                        }";
            return new List<ITemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }

        IEnumerable<IPersistentClassInfo> GetReferenceMembers(IPersistentClassInfo classInfo, string customer, string order, string orderLine) {
            if (classInfo.Name == order) {
                return new List<IPersistentClassInfo> { classInfo.PersistentAssemblyInfo.PersistentClassInfos.Single(info => info.Name == customer) };
            }
            if (classInfo.Name == orderLine) {
                return new List<IPersistentClassInfo> { classInfo.PersistentAssemblyInfo.PersistentClassInfos.Single(info => info.Name == order) };
            }
            return new List<IPersistentClassInfo>();
        }


        IEnumerable<string> GetClassNames(string customer, string order, string orderLine) {
            return new[] { customer, order, orderLine };
        }

        Type GetInheritance(IPersistentClassInfo info) {
            if (info.Name.ToLower().IndexOf("customer", StringComparison.Ordinal) > -1)
                return typeof(CustomerBase);
            if (info.Name.ToLower().EndsWith("order"))
                return typeof(OrderBase);
            return typeof(OrderLineBase);
        }

        public IPersistentAssemblyInfo Build(string customer, string order, string orderLine, string masterDetailDynamicAssembly) {
            var objectSpace = new XPObjectSpace(XafTypesInfo.Instance, XpandModuleBase.XpoTypeInfoSource, () => new UnitOfWork(_session.DataLayer));
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(objectSpace, masterDetailDynamicAssembly);
            IClassInfoHandler classInfoHandler = persistentAssemblyBuilder.CreateClasses(GetClassNames(customer, order, orderLine));
            classInfoHandler.CreateTemplateInfos(persistentClassInfo => GetTemplateInfos(persistentClassInfo, customer, order));
            classInfoHandler.SetInheritance(info => GetInheritance(info));
            classInfoHandler.CreateReferenceMembers(classInfo => GetReferenceMembers(classInfo, customer, order, orderLine), true);
            objectSpace.CommitChanges();
            return persistentAssemblyBuilder.PersistentAssemblyInfo;
        }

    }
}