using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public class AssemblyGenerator {
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        readonly IObjectSpace _objectSpace;
        readonly LogonObject _logonObject;
        readonly DBTable[] _storageTables;


        public AssemblyGenerator(LogonObject logonObject, IPersistentAssemblyInfo persistentAssemblyInfo, string[] tables) {
            _persistentAssemblyInfo = persistentAssemblyInfo;
            var dataStoreSchemaExplorer = ((IDataStoreSchemaExplorer)XpoDefault.GetConnectionProvider(logonObject.ConnectionString, AutoCreateOption.None));
            _storageTables = dataStoreSchemaExplorer.GetStorageTables(tables);
            _logonObject = logonObject;
            _objectSpace = ObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
        }

        public void Create() {
            Dictionary<string, ClassGeneratorInfo> generatorInfos = new ClassGenerator(_persistentAssemblyInfo, _storageTables).CreateAll().ToDictionary(classGeneratorInfo => classGeneratorInfo.PersistentClassInfo.Name);
            foreach (var classGeneratorInfo in generatorInfos.Where(pair => pair.Value.PersistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Class)) {
                var generatorInfo = classGeneratorInfo.Value;
                new ClassAtrributeGenerator(generatorInfo, _logonObject.NavigationPath).Create().Each(info => generatorInfo.PersistentClassInfo.TypeAttributes.Add(info));
                var memberGeneratorInfos = new MemberGenerator(classGeneratorInfo.Value.DbTable, generatorInfos).Create();
                memberGeneratorInfos.Each(info => new MemberAttributeGenerator(info, generatorInfo).Create());
            }
            var oneToOneMemberInfos = _persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers.OfType<IPersistentReferenceMemberInfo>()).Where(info => info.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember);
            foreach (var oneToOneMemberInfo in oneToOneMemberInfos) {
                var codeTemplate = _objectSpace.CreateWCObject<ICodeTemplate>();
                codeTemplate.TemplateCode = oneToOneMemberInfo.ReferenceClassInfo.OwnMembers.OfType<IPersistentReferenceMemberInfo>().Where(info => info.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember).Single().Name;
                oneToOneMemberInfo.TemplateInfos.Add(codeTemplate);
            }
            CodeEngine.SupportCompositeKeyPersistentObjects(_persistentAssemblyInfo);
            CreateAssemblyAttributes();
        }


        void CreateAssemblyAttributes() {
            if (_persistentAssemblyInfo.PersistentClassInfos.Count() > 0) {
                var persistentAssemblyDataStoreAttributeInfo =
                    _objectSpace.CreateWCObject<IPersistentAssemblyDataStoreAttribute>();
                persistentAssemblyDataStoreAttributeInfo.ConnectionString = _logonObject.ConnectionString;
                persistentAssemblyDataStoreAttributeInfo.PersistentClassInfo = _persistentAssemblyInfo.PersistentClassInfos[0];
                _persistentAssemblyInfo.Attributes.Add(persistentAssemblyDataStoreAttributeInfo);
            }
        }
    }

}