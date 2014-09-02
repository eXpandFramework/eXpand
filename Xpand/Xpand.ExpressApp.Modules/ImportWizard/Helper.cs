﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ImportWizard.Properties;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.ImportWizard {
    public static class Helper {

        /// <summary>
        /// Copied from DevExpress.ExpressApp.SystemModule.NewObjectViewController
        /// </summary>
        /// <param name="vc">ViewController</param>
        public static CollectionSourceBase GetCurrentCollectionSource(this ViewController vc) {
            PropertyCollectionSourceLink propertyCollectionSourceLink = null;
            CollectionSourceBase result = null;
            var listView = vc.View as ListView;
            if (listView != null)
                result = listView.CollectionSource;
            else {
                var linkToListViewController = vc.Frame.GetController<LinkToListViewController>();
                var hasLink = (linkToListViewController != null) && (linkToListViewController.Link != null);
                if (hasLink) {
                    if (linkToListViewController.Link.ListView != null)
                        result = linkToListViewController.Link.ListView.CollectionSource;
                    propertyCollectionSourceLink = linkToListViewController.Link.PropertyCollectionSourceLink;
                }
            }
            if (result == null) {
                if (propertyCollectionSourceLink != null) {
                    throw new NotImplementedException(Resources.Helper_GetCurrentCollectionSource_Bad_Extention_method_for_ViewController__See_TP_Shell_XAF_Module_Win_Extentions_GetCurrentCollectionSource_for_details__);
                }
            }
            return result;
        }


        [Localizable(false)]
        public static XPBaseObject GetXpObjectByKeyValue(XPObjectSpace oSpace, string value, Type type) {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!type.IsSubclassOf(typeof(XPBaseObject)))
                return null;

            var defaultMember = GetDefaultMember(type);
            object result;
            value.TryToChange(defaultMember.MemberType, out result);
            var criteriaOperator = GetDefaultCriteria(defaultMember, result);
            var item = (XPBaseObject)oSpace.FindObject(type,criteriaOperator,true);
            if (item != null)
                return item;

            var nestedObjectSpace = oSpace.CreateNestedObjectSpace();
            item = (XPBaseObject)nestedObjectSpace.CreateObject(type);
            defaultMember.SetValue(item, value);

            item.Save();
            nestedObjectSpace.CommitChanges();

            return oSpace.GetObject(item);

        }

        private static CriteriaOperator GetDefaultCriteria(IMemberInfo memberInfo, object value){
            var criteria = CriteriaOperator.Parse(((IModelClassDefaultCriteria)memberInfo.Owner.ModelClass()).DefaultCriteria);
            var dictionary = new Dictionary<string, object>{{memberInfo.Name, value}};
            new BinaryOperatorValueMapperCriteriaProcessor(dictionary).Process(criteria);
            return criteria;
        }

        private static IMemberInfo GetDefaultMember(Type type){
            var typeInfo = XafTypesInfo.CastTypeToTypeInfo(type);
            var defaultMember = typeInfo.GetDefaultMember();
            return defaultMember;
        }


        public static XPBaseObject GetXpObjectByKeyValue(UnitOfWork uow, string value, Type type, string prop) {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!type.IsSubclassOf(typeof(XPBaseObject)))
                return null;

            //  var keyPropertyName = prop;          

            var item = (XPBaseObject)uow.FindObject(type,new BinaryOperator(prop, value),true);
            if (item != null)
                return item;

            //var nestedUow = uow.BeginNestedUnitOfWork();
            item = (XPBaseObject)type.CreateInstance(uow);
            var firstOrDefault = item.ClassInfo.PersistentProperties.OfType<XPMemberInfo>().FirstOrDefault(p => p.Name == prop);
            if (firstOrDefault != null)
                firstOrDefault.SetValue(item, value);

            item.Save();
            //uow.CommitChanges();

            return item;


        }
    }
}
