using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.BusinessObjects {
    public static class AssemblyInfoExtensions {
        public static Version Version(this IPersistentAssemblyInfo info) {
            var xpandVersion = new Version(XpandAssemblyInfo.Version);
            return new Version(xpandVersion.Major, xpandVersion.Minor,
                int.Parse(xpandVersion.Build.ToString() + xpandVersion.Revision), info.Revision);
        }

        public static void CreateMissingAssociations(this IPersistentAssemblyInfo assemblyInfo){
            var attributes =
                assemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers)
                    .SelectMany(info => info.TypeAttributes)
                    .OfType<IPersistentAssociationAttribute>()
                    .Where(attribute => attribute.RelationType != RelationType.Undefined);
            foreach (var attribute in attributes){
                if (attribute.RelationType == RelationType.OneToMany){
                    var persistentReferenceMemberInfo = attribute.Owner as IPersistentReferenceMemberInfo;
                    if (persistentReferenceMemberInfo != null)
                        persistentReferenceMemberInfo.GetAssociatedCollection(attribute.ElementTypeFullName);
                    else{
                        ((IPersistentCollectionMemberInfo) attribute.Owner).GetAssociatedReference(attribute.ElementTypeFullName);
                    }
                }
            }
        }

        public static IPersistentClassInfo CreateClass(this IPersistentAssemblyInfo assemblyInfo, string className){
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>();
            var behavior = PersistentCriteriaEvaluationBehavior.InTransaction;
            var persistentClassInfo =(IPersistentClassInfo)assemblyInfo.Session.FindObject(behavior, objectType,
                        CriteriaOperator.Parse(nameof(IPersistentClassInfo.Name) +"=?", className)) ??
                (IPersistentClassInfo) Activator.CreateInstance(objectType, assemblyInfo.Session);
            persistentClassInfo.Name = className;
            persistentClassInfo.PersistentAssemblyInfo = assemblyInfo;
            assemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
            return persistentClassInfo;
        }

    }
}

