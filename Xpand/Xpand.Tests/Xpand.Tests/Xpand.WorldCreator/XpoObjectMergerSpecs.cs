using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Utils;

namespace Xpand.Tests.Xpand.WorldCreator {
    [Subject(typeof (XpoObjectMerger))]
    [Isolated]
    public class When_Merging_Dynamic_Types : With_In_Memory_DataStore {
        static readonly List<Type> types = new List<Type>();
        static IDbCommand _dbCommand;
        static bool execCommand;

        static int updateSchemaCalls;
        static XpoObjectMerger xpoObjectMerger;

        Establish context = () => {
            xpoObjectMerger = new XpoObjectMerger();

            new User(UnitOfWork).Save();
            var persistentClassInfo = new PersistentClassInfo(UnitOfWork)
                                      {Name = "Parent", BaseType = typeof (User), MergedObjectType = typeof (User)};
            Assembly.GetAssembly(typeof (ObjectMerger));
            var codeTemplate = new CodeTemplate(UnitOfWork) {TemplateType = TemplateType.Class};
            codeTemplate.SetDefaults();
            persistentClassInfo.CodeTemplateInfo=new CodeTemplateInfo(UnitOfWork) {TemplateInfo = codeTemplate};
            persistentClassInfo.PersistentAssemblyInfo = new PersistentAssemblyInfo(UnitOfWork)
                                                         {Name = "MergingAssembly"};
            persistentClassInfo.Save();

            UnitOfWork.CommitChanges();
            Type compileModule = new CompileEngine().CompileModule(persistentClassInfo.PersistentAssemblyInfo, Path.GetDirectoryName(Application.ExecutablePath));
            types.AddRange(compileModule.Assembly.GetTypes().Where(type => !typeof (ModuleBase).IsAssignableFrom(type)));
            types.Add(typeof (User));

            _dbCommand = Isolate.Fake.Instance<IDbCommand>();
            Isolate.WhenCalled(() => _dbCommand.ExecuteNonQuery()).DoInstead(callContext => {
                execCommand = true;
                return 0;
            });
        };

        Because of = () => xpoObjectMerger.MergeTypes(UnitOfWork, types, _dbCommand);


        It should_call_an_update_sql_statement = () => execCommand.ShouldEqual(true);

        It should_create_an_ObjectType_Column_to_Parent_if_parent_has_no_records =
            () => UnitOfWork.GetClassInfo(types[0]).FindMember("ObjectType").ShouldNotBeNull();
    }
}