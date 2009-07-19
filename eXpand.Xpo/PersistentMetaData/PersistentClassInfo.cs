using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.PersistentMetaData
{
    public class PersistentClassInfo : PersistentTypeInfo {
        public static void FillDictionary(XPDictionary dictionary, ICollection<PersistentClassInfo> data) {
            foreach(PersistentClassInfo twc in data) {
                twc.CreateClass(dictionary);
            }
            foreach(PersistentClassInfo twc in data) {
                twc.CreateMembers(dictionary);
            }
        }

        public const string AssemblyName = "";

        public PersistentClassInfo()
        {
        }

        public PersistentClassInfo(Session session) : base(session) { }

        protected virtual Type GetDefaultBaseClass() {
            return typeof(MyBaseObject);
        }

        public XPClassInfo CreateClass(XPDictionary dictionary) {
            XPClassInfo result = dictionary.QueryClassInfo(AssemblyName, Name);
            if(result == null) {
                XPClassInfo baseClassInfo = BaseClass != null ? BaseClass.CreateClass(dictionary) : dictionary.GetClassInfo(GetDefaultBaseClass());
                result = dictionary.CreateClass(baseClassInfo, Name);
                CreateAttributes(result);
            }
            return result;
        }

        void CreateMembers(XPDictionary dictionary) {
            XPClassInfo ci = dictionary.GetClassInfo(AssemblyName, Name);
            foreach(PersistentMemberInfo mi in OwnMembers) {
                mi.CreateMember(ci);
            }
        }

        PersistentClassInfo _BaseClass;
        public PersistentClassInfo BaseClass {
            get { return _BaseClass; }
            set { SetPropertyValue("BaseClass", ref _BaseClass, value); }
        }

        [Association]
        public XPCollection<PersistentMemberInfo> OwnMembers { get { return GetCollection<PersistentMemberInfo>("OwnMembers"); } }

        public static PersistentClassInfo CreateOneToMany(PersistentClassInfo onePart, string manyPart)
        {
            
            var persistentAssociationAttribute = new PersistentAssociationAttribute { ElementTypeName = manyPart };
            var persistentCollectionMemberInfo = new PersistentCollectionMemberInfo(persistentAssociationAttribute) { Name = manyPart + "s" };
            persistentCollectionMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            onePart.OwnMembers.Add(persistentCollectionMemberInfo);
            

            PersistentClassInfo manyPartInfo = CreateMany(onePart, manyPart);

            
            return manyPartInfo;
        }
        public static List<PersistentClassInfo> CreateOneToMany(string onePart, string manyPart)
        {
            var classInfos = new List<PersistentClassInfo>();
            var customerClass = new PersistentClassInfo(Session.DefaultSession) { Name = onePart };
            classInfos.Add(customerClass);
            classInfos.Add(CreateOneToMany(customerClass, manyPart));
            return classInfos;
        }

        private static PersistentClassInfo CreateMany(PersistentClassInfo persistentClassInfo, string manyPart)
        {
            var manyPartClassInfo = new PersistentClassInfo { Name = manyPart };
            var orderPersistentAssociationAttribute = new PersistentAssociationAttribute();
            var customerReferenceMemberInfo = new PersistentReferenceMemberInfo { Name = persistentClassInfo.Name, ReferenceType = persistentClassInfo };
            customerReferenceMemberInfo.TypeAttributes.Add(orderPersistentAssociationAttribute);
            manyPartClassInfo.OwnMembers.Add(customerReferenceMemberInfo);
            return manyPartClassInfo;
        }
    }
}