using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "Name,Assembly")]
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    [InterfaceRegistrator(typeof(IInterfaceInfo))]
    public class InterfaceInfo : BaseObject, IInterfaceInfo {
        string _assembly;
        string _name;

        public InterfaceInfo(Session session)
            : base(session) {
        }

        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<PersistentClassInfo> PersistentClassInfos {
            get { return GetCollection<PersistentClassInfo>("PersistentClassInfos"); }
        }
        #region IInterfaceInfo Members
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public string Assembly {
            get { return _assembly; }
            set { SetPropertyValue("Assembly", ref _assembly, value); }
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public Type Type {
            get {
                Assembly singleOrDefault =
                    AppDomain.CurrentDomain.GetAssemblies().Where(
                        assembly => new AssemblyName(assembly.FullName + "").Name == Assembly).SingleOrDefault();
                if (singleOrDefault != null)
                    return singleOrDefault.GetType(Name);
                return null;
            }
        }

        IList<IPersistentClassInfo> IInterfaceInfo.PersistentClassInfos {
            get { return new ListConverter<IPersistentClassInfo, PersistentClassInfo>(PersistentClassInfos); }
        }
        #endregion
    }
}