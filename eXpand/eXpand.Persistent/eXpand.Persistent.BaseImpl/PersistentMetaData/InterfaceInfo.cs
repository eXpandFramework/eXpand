using System;
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> CodeDomApproachForWorldCreator
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

<<<<<<< HEAD
namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleCombinationOfPropertiesIsUnique(null,DefaultContexts.Save, "Name,Assembly")]
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class InterfaceInfo:BaseObject, IInterfaceInfo {
        public InterfaceInfo(Session session) : base(session) {
        }
        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<PersistentClassInfo> PersistentClassInfos
        {
            get
            {
                return GetCollection<PersistentClassInfo>("PersistentClassInfos");
            }
        }
        
=======
namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "Name,Assembly")]
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class InterfaceInfo : BaseObject, IInterfaceInfo
    {
        public InterfaceInfo(Session session)
            : base(session)
        {
        }

>>>>>>> CodeDomApproachForWorldCreator
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private string _assembly;
        public string Assembly
        {
            get
            {
                return _assembly;
            }
            set
            {
                SetPropertyValue("Assembly", ref _assembly, value);
            }
        }
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
<<<<<<< HEAD
        public Type Type {
            get {
                var singleOrDefault = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => new AssemblyName(assembly.FullName + "").Name == Assembly).SingleOrDefault();
                if (singleOrDefault!= null)
=======
        public Type Type
        {
            get
            {
                var singleOrDefault = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => new AssemblyName(assembly.FullName + "").Name == Assembly).SingleOrDefault();
                if (singleOrDefault != null)
>>>>>>> CodeDomApproachForWorldCreator
                    return singleOrDefault.GetType(Name);
                return null;
            }
        }
<<<<<<< HEAD
=======

        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<PersistentClassInfo> PersistentClassInfos {
            get { return GetCollection<PersistentClassInfo>("PersistentClassInfos"); }
        }

        IList<IPersistentClassInfo> IInterfaceInfo.PersistentClassInfos {
            get { return new ListConverter<IPersistentClassInfo,PersistentClassInfo>(PersistentClassInfos); }
        }

>>>>>>> CodeDomApproachForWorldCreator
    }
}