using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using FeatureCenter.Base;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Utils.Helpers;

namespace FeatureCenter.Module.Win.WorldCreator.DynamicAssemblyMasterDetail {
    public class DynamicAssemblyBuilder {
        private readonly IObjectSpace _objectSpace;


        public DynamicAssemblyBuilder(IObjectSpace objectSpace){
            _objectSpace = objectSpace;
        }

        List<TemplateInfo> GetOrderLineTemplateInfos(IPersistentClassInfo info, string order) {
            string code = @"protected override void SetOrder(" + typeof(IOrder).FullName + @" order){
                               " + order + @" = (" + order + @")order;
                        }
                        protected override " + typeof(IOrder).FullName + @" GetOrder() {
                            return " + order + @";
                        }";
            return new List<TemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }

        List<TemplateInfo> GetOrderTemplateInfos(IPersistentClassInfo info, string customer) {
            string code = @"protected override void SetCustomer(" + typeof(ICustomer).FullName + @" customer){
                               " + customer + @" = (" + customer + @")customer;
                        }
                        protected override " + typeof(ICustomer).FullName + @" GetCustomer() {
                            return " + customer + @";
                        }";
            return new List<TemplateInfo> { new TemplateInfo(info.Session) { TemplateCode = code } };
        }


        Type GetInheritance(IPersistentClassInfo info) {
            if (info.Name.ToLower().IndexOf("customer", StringComparison.Ordinal) > -1)
                return typeof(CustomerBase);
            if (info.Name.ToLower().EndsWith("order"))
                return typeof(OrderBase);
            return typeof(OrderLineBase);
        }

        public PersistentAssemblyInfo Build(string customer, string order, string orderLine, string name) {
            var persistentAssemblyInfo = _objectSpace.CreateObject<PersistentAssemblyInfo>();
            persistentAssemblyInfo.Name = name;

            var customerClassInfo = persistentAssemblyInfo.CreateClass(customer);
            customerClassInfo.BaseTypeFullName = GetInheritance(customerClassInfo).FullName;

            var orderClassInfo = persistentAssemblyInfo.CreateClass(order);
            orderClassInfo.BaseTypeFullName = GetInheritance(orderClassInfo).FullName;
            orderClassInfo.CreateReferenceMember(customerClassInfo,true);
            GetOrderTemplateInfos(orderClassInfo,customer).Each(info => orderClassInfo.TemplateInfos.Add(info));
            

            var orderLineClassInfo = persistentAssemblyInfo.CreateClass(orderLine);
            orderLineClassInfo.BaseTypeFullName = GetInheritance(orderLineClassInfo).FullName;
            orderLineClassInfo.CreateReferenceMember(orderClassInfo,true);
            GetOrderLineTemplateInfos(orderLineClassInfo, order).Each(info => orderLineClassInfo.TemplateInfos.Add(info));

            _objectSpace.CommitChanges();
            return persistentAssemblyInfo;
        }

    }
}