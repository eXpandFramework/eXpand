using System;
using System.IO;
using System.Windows.Forms;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Machine.Specifications;
using System.Linq;

namespace Xpand.Tests.Xpand.WorldCreator {
    public class When_validating_script : With_In_Memory_DataStore {
        static PersistentAssemblyInfo _info;

        Establish context = () => {
            _info = XPObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _info.Name = "TestAssemlby";
        };

        Because of = () => _info.Validate(Path.GetDirectoryName(Application.ExecutablePath));


        It should_not_generate_assembly_in_memory =
            () =>
            AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => (assembly.FullName + "").StartsWith("TestAssemlby")).ShouldBeNull();

        It should_save_errors_at_persistentAssemblyInfo_compile_errors = () => _info.CompileErrors.ShouldBeNull();

    }

}
