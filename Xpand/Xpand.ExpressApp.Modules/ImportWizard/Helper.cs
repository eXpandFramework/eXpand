﻿using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.ImportWizard.Properties;

namespace Xpand.ExpressApp.ImportWizard
{
    public static class Helper
    {

        /// <summary>
        /// Copied from DevExpress.ExpressApp.SystemModule.NewObjectViewController
        /// </summary>
        /// <param name="vc">ViewController</param>
        public static CollectionSourceBase GetCurrentCollectionSource(this ViewController vc)
        {
            PropertyCollectionSourceLink propertyCollectionSourceLink = null;
            CollectionSourceBase result = null;
            if (vc.View is ListView)
                result = ((ListView)vc.View).CollectionSource;
            else
            {
                var linkToListViewController = vc.Frame.GetController<LinkToListViewController>();
                var hasLink = (linkToListViewController != null) && (linkToListViewController.Link != null);
                if (hasLink)
                {
                    if (linkToListViewController.Link.ListView != null)
                        result = linkToListViewController.Link.ListView.CollectionSource;
                    propertyCollectionSourceLink = linkToListViewController.Link.PropertyCollectionSourceLink;
                }
            }
            if (result == null)
            {
                if (propertyCollectionSourceLink != null)
                {
                    throw new NotImplementedException(Resources.Helper_GetCurrentCollectionSource_Bad_Extention_method_for_ViewController__See_TP_Shell_XAF_Module_Win_Extentions_GetCurrentCollectionSource_for_details__);
                }
            }
            return result;
        }


        [Localizable(false)]
        public static XPBaseObject GetXpObjectByKeyValue(ObjectSpace oSpace, string value, Type type)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!type.IsSubclassOf(typeof(XPBaseObject)))
                return null;

            var keyPropertyName = oSpace.Session.GetClassInfo(type).
                        PersistentProperties.
                        OfType<XPMemberInfo>().
                        Where(p => p.HasAttribute(typeof(KeyPropertyAttribute))).
                        Select(p => p.Name).
                        FirstOrDefault() ??

                    oSpace.Session.GetClassInfo(type).
                        PersistentProperties.
                        OfType<XPMemberInfo>().
                        Where(p => p.Name == "Name" || p.Name == "Code")
                        .Select(p => p.Name)
                        .FirstOrDefault() ??
                    "Oid";

            var item = (XPBaseObject)oSpace.FindObject(
                                type,
                                new BinaryOperator(keyPropertyName, value),
                                true);
            if (item != null)
                return item;

            var nestedObjectSpace = oSpace.CreateNestedObjectSpace();
            item = (XPBaseObject)nestedObjectSpace.CreateObject(type);
            var firstOrDefault = item.ClassInfo
                                    .PersistentProperties
                                    .OfType<XPMemberInfo>()
                                    .FirstOrDefault(p => p.Name == keyPropertyName);
            if (firstOrDefault != null)
                firstOrDefault.SetValue(item, value);

            item.Save();
            nestedObjectSpace.CommitChanges();

            return oSpace.GetObject(item);

        }


        public static XPBaseObject GetXpObjectByKeyValue(UnitOfWork uow, string value, Type type, string prop)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (!type.IsSubclassOf(typeof(XPBaseObject)))
                return null;

            //  var keyPropertyName = prop;          

            var item = (XPBaseObject)uow.FindObject(
                                type,
                                new BinaryOperator(prop, value),
                                true);
            if (item != null)
                return item;

            //var nestedUow = uow.BeginNestedUnitOfWork();
            item = (XPBaseObject)ReflectionHelper.CreateObject(type, uow);
            var firstOrDefault = item.ClassInfo
                                    .PersistentProperties
                                    .OfType<XPMemberInfo>()
                                    .FirstOrDefault(p => p.Name == prop);
            if (firstOrDefault != null)
                firstOrDefault.
                    SetValue(item, value);

            item.Save();
            //uow.CommitChanges();

            return item;


        }

    }
}
