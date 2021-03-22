using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using Xpand.Extensions.StringExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.AssemblyGenerator {
    public class ClassAtrributeGenerator {
        readonly IPersistentClassInfo _persistentClassInfo;
        readonly DBTable _dbTable;
        readonly string _navigationPath;
        readonly IObjectSpace _objectSpace;

        public ClassAtrributeGenerator(ClassGeneratorInfo classGeneratorInfo, string navigationPath) {
            _persistentClassInfo = classGeneratorInfo.PersistentClassInfo;
            _dbTable = classGeneratorInfo.DbTable;
            _navigationPath = navigationPath;
            _objectSpace = XPObjectSpace.FindObjectSpaceByObject(_persistentClassInfo);
        }

        public IEnumerable<IPersistentAttributeInfo> Create() {
            var persistentAttributeInfos = new List<IPersistentAttributeInfo>();
            if (_persistentClassInfo.TypeAttributes.OfType<IPersistentPersistentAttribute>().FirstOrDefault() == null)
                persistentAttributeInfos.Add(GetPersistentPersistentAttribute(_dbTable.Name));
            if (!(string.IsNullOrEmpty(_navigationPath)) && _persistentClassInfo.TypeAttributes.OfType<IPersistentNavigationItemAttribute>().FirstOrDefault() == null) {
                var persistentNavigationItemAttribute = _objectSpace.Create<IPersistentNavigationItemAttribute>();
                var cleanName = _persistentClassInfo.Name.CleanCodeName();
                persistentNavigationItemAttribute.Path = _navigationPath + "/" + cleanName;
                persistentNavigationItemAttribute.ViewId = cleanName + "_ListView";
                persistentAttributeInfos.Add(persistentNavigationItemAttribute);
            }
            return persistentAttributeInfos;
        }

        IPersistentPersistentAttribute GetPersistentPersistentAttribute(string name) {
            var persistentPersistentAttribute = _objectSpace.Create<IPersistentPersistentAttribute>();
            persistentPersistentAttribute.MapTo = name;
            return persistentPersistentAttribute;
        }

    }
}